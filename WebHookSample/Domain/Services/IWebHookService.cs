using WebHookSample.Resources.DTOs.WebHook.Request;
using WebHookSample.Resources.DTOs.WebHook.Response;
using WebHookSample.Resources.Enums;

namespace WebHookSample.Domain.Services;

public interface IWebHookService
{
    /// <summary>
    /// Role: create webhook
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<BaseResult<WebHookResponse>> CreateAsync(CreateWebHookRequest request, CancellationToken token);

    /// <summary>
    /// Role: the classification of webhook can be performed immediately
    /// </summary>
    /// <param name="triggerDatetimeUtc"></param>
    /// <param name="utcNow"></param>
    /// <returns></returns>
    (ExecutionLevel level, double seconds) GetExecutionLevel(DateTime triggerDatetimeUtc, DateTime utcNow);

    Task RequestSoonAsync(Models.WebHook request, CancellationToken token);
}