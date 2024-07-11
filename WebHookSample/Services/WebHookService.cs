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

    public async Task<BaseResult<WebHookResponse>> CreateAsync(CreateWebHookRequest request, CancellationToken token)
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
        await Context.WebHooks.AddAsync(webHook, token);

        // Classification job
        var executeNow = GetExecutionLevel(request.TriggerDatetimeUtc, DateTime.UtcNow);

        if (executeNow.level == ExecutionLevel.Now)
            await RequestNowAsync(webHook, token);

        if (executeNow.level == ExecutionLevel.Soon)
        {
            webHook.Level = ExecutionLevel.Soon;
            await Context.SaveChangesAsync(token);
            webHook.TimeEvents = null!; // Avoid circle loop when json-serialization
            BackgroundJob.Schedule(() => RequestSoonAsync(webHook, token), TimeSpan.FromSeconds(executeNow.seconds));
        }

        if (executeNow.level == ExecutionLevel.Later)
        {
            webHook.Level = ExecutionLevel.Later;
            await Context.SaveChangesAsync(token);
        }

        return GetBaseResult(CodeMessage._99, Mapper.Map<WebHookResponse>(webHook));
    }

    /// <summary>
    /// Role: the classification of webhook can be performed immediately
    /// </summary>
    /// <param name="triggerDatetimeUtc"></param>
    /// <param name="utcNow"></param>
    /// <returns></returns>
    public (ExecutionLevel level, int seconds) GetExecutionLevel(DateTime triggerDatetimeUtc, DateTime utcNow)
    {
        var condition = triggerDatetimeUtc - utcNow;

        if (condition.TotalSeconds <= 15)
            return (ExecutionLevel.Now, 0);
        if (condition.TotalHours <= 1)
            return (ExecutionLevel.Soon, (int)condition.TotalSeconds);

        return (ExecutionLevel.Later, 0);
    }

    public async Task RequestSoonAsync(Models.WebHook request, CancellationToken token)
    {
        var level = await customHttpClient.SendAsync(request, token);

        request.IsDone = true;
        Models.TimeEvent timeEvent = new()
        {
            ProcessType = level,
            TimeStampUtc = DateTime.UtcNow,
            WebHookId = request.Id
        };

        await Context.TimeEvents.AddAsync(timeEvent, token);
        await Context.SaveChangesAsync(token);
    }

    #region Private work

    private async Task RequestNowAsync(Models.WebHook request, CancellationToken token)
    {
        var level = await customHttpClient.SendAsync(request, token);

        request.IsDone = true;
        request.Level = ExecutionLevel.Now;
        request.TimeEvents.Add(
            new()
            {
                ProcessType = level,
                TimeStampUtc = DateTime.UtcNow,
            }
        );

        await Context.SaveChangesAsync(token);
    }

    #endregion

    #endregion
}