using WebHookSample.Resources.Enums;

namespace WebHookSample.Domain.Services;

public interface ICustomHttpClient
{
    /// <summary>
    /// Role: send request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ProcessType> SendAsync(Models.WebHook request, CancellationToken cancellationToken);
}