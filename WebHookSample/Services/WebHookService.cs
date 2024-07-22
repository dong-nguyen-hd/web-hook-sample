using AutoMapper;
using Hangfire;
using WebHookSample.Domain.Context;
using WebHookSample.Domain.Services;
using WebHookSample.Resources.DTOs.WebHook.Request;
using WebHookSample.Resources.DTOs.WebHook.Response;
using WebHookSample.Resources.Enums;

namespace WebHookSample.Services;

public sealed class WebHookService(IMapper mapper, CoreContext context, ICustomHttpClient customHttpClient) : BaseService(mapper, context), IWebHookService
{
    #region Method

    public async Task<BaseResult<WebHookResponse>> CreateAsync(CreateWebHookRequest request, CancellationToken cancellationToken = default)
    {
        // Save to DB
        var webHook = Mapper.Map<Models.WebHook>(request);
        webHook.TimeEvents = new HashSet<Models.TimeEvent>()
        {
            new()
            {
                ProcessType = ProcessType.Init,
                TimeStampUtc = DateTime.UtcNow,
            }
        };
        await Context.WebHooks.AddAsync(webHook, cancellationToken);

        // Classification job
        var executeNow = GetExecutionLevel(request.TriggerDatetimeUtc, DateTime.UtcNow);

        if (executeNow.level == ExecutionLevel.Now)
            await RequestNowAsync(webHook, cancellationToken);

        if (executeNow.level == ExecutionLevel.Soon)
        {
            webHook.Level = ExecutionLevel.Soon;
            await Context.SaveChangesAsync(cancellationToken);
            webHook.TimeEvents = null!; // Avoid circle loop when json-serialization
            BackgroundJob.Schedule(() => RequestSoonAsync(webHook, cancellationToken), TimeSpan.FromSeconds(executeNow.seconds));
        }

        if (executeNow.level == ExecutionLevel.Later)
        {
            webHook.Level = ExecutionLevel.Later;
            await Context.SaveChangesAsync(cancellationToken);
        }

        return GetBaseResult(CodeMessage._99, Mapper.Map<WebHookResponse>(webHook));
    }

    public (ExecutionLevel level, double seconds) GetExecutionLevel(DateTime triggerDatetimeUtc, DateTime utcNow)
    {
        var condition = triggerDatetimeUtc - utcNow;

        if (condition.TotalSeconds <= 15)
            return (ExecutionLevel.Now, 0);
        if (condition.TotalHours <= 1)
            return (ExecutionLevel.Soon, condition.TotalSeconds);

        return (ExecutionLevel.Later, 0);
    }

    public async Task RequestSoonAsync(Models.WebHook request, CancellationToken cancellationToken = default)
    {
        var level = await customHttpClient.SendAsync(request, cancellationToken);

        request.IsDone = true;
        Models.TimeEvent timeEvent = new()
        {
            ProcessType = level,
            TimeStampUtc = DateTime.UtcNow,
            WebHookId = request.Id
        };

        Context.WebHooks.Update(request);
        await Context.TimeEvents.AddAsync(timeEvent, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    #region Private work

    private async Task RequestNowAsync(Models.WebHook request, CancellationToken cancellationToken = default)
    {
        var level = await customHttpClient.SendAsync(request, cancellationToken);

        request.IsDone = true;
        request.Level = ExecutionLevel.Now;
        request.TimeEvents.Add(
            new()
            {
                ProcessType = level,
                TimeStampUtc = DateTime.UtcNow,
            }
        );

        await Context.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #endregion
}