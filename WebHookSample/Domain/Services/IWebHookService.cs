using WebHookSample.Resources.DTOs.WebHook.Request;
using WebHookSample.Resources.DTOs.WebHook.Response;

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
    /// Role: request to 3th
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task RequestAsync(Models.WebHook request, CancellationToken token);
}