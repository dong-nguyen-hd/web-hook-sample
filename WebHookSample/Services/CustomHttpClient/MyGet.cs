namespace WebHookSample.Services.CustomHttpClient;

public sealed partial class CustomHttpClient
{
    public async Task<bool> Get(Models.WebHook request, CancellationToken ts)
    {
        var log = GetLog(request);

        try
        {
            // Set up client
            var client = request.EnableVerifyTls ? httpClientFactory.CreateClient(RelateHttpClient.EnableTLS) : httpClientFactory.CreateClient(RelateHttpClient.DisableTLS);
            SetHeader(client, request, log);

            // Request
            var response = await client.GetAsync(request.Uri, ts);

            return true;
        }
        catch (Exception ex)
        {
            log.HasException = true;
            log.ExceptionMessage = ex.Message;
            log.ExceptionStackTrace = ex.StackTrace;

            return false;
        }
        finally
        {
            logService.Write(log);
        }
    }
}