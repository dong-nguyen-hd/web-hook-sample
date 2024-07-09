namespace WebHookSample.Domain.Services;

public interface ICustomHttpClient
{
    Task<bool> Post(Models.WebHook request, CancellationToken ts);
}