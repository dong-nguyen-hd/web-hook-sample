using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
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
        await Context.WebHooks.AddAsync(webHook, token);
        await Context.SaveChangesAsync(token);

        var executeNow = IsExecuteNow(request.TriggerDatetimeUtc, DateTime.UtcNow);
        if (executeNow.isNow)
            BackgroundJob.Schedule(() => RequestAsync(webHook, token), TimeSpan.FromSeconds(executeNow.seconds));

        return GetBaseResult(CodeMessage._99, Mapper.Map<WebHookResponse>(webHook));
    }

    #region Private work

    public async Task RequestAsync(Models.WebHook request, CancellationToken token)
    {
        bool isSuccess = await customHttpClient.Post(request, token);

        // Save time event
        var webHook = await Context.WebHooks.SingleAsync(x => x.Id == request.Id);
        webHook.TimeEvents = new HashSet<Models.TimeEvent>()
        {
            new()
            {
                ProcessType = isSuccess ? ProcessType.Success : ProcessType.Fail,
                TimeStampUtc = DateTime.UtcNow,
            }
        };
        await Context.SaveChangesAsync();
    }

    /// <summary>
    /// Role: the classification of webhook can be performed immediately
    /// </summary>
    /// <param name="triggerDatetimeUtc"></param>
    /// <param name="utcNow"></param>
    /// <returns></returns>
    private (bool isNow, int seconds) IsExecuteNow(DateTime triggerDatetimeUtc, DateTime utcNow)
    {
        var condition = utcNow - triggerDatetimeUtc;
        int seconds = condition.TotalSeconds < 0 ? 0 : (int)condition.TotalSeconds;

        if (condition.TotalHours < 1)
            return (true, seconds);

        return (false, 0);
    }

    #endregion

    #endregion
}