using System.Text;

namespace WebHookSample.Services.CustomHttpClient;

public sealed partial class CustomHttpClient
{
    public async Task<bool> Post(Models.WebHook request, CancellationToken ts)
    {
        var log = GetLog(request);

        try
        {
            // Set up client
            var client = request.EnableVerifyTls ? httpClientFactory.CreateClient(RelateHttpClient.EnableTLS) : httpClientFactory.CreateClient(RelateHttpClient.DisableTLS);
            SetHeader(client, request, log);

            // Request
            var payload = new StringContent(request.Payload ?? string.Empty, Encoding.UTF8);
            var response = await client.PostAsync(request.Uri, payload, ts);
            SetLogResponse(log, response);

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