using WebHookSample.Resources.DTOs.WebHook.Request;
using WebHookSample.Resources.DTOs.WebHook.Response;

namespace WebHookSample.Domain.Services;

public interface IWebHookService
{
    /// <summary>
    /// Role: create webhook
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<WebHookResponse> CreateAsync(CreateWebHookRequest request);
}