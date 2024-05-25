namespace WebHookSample.Extensions.AddConfig;

using Polly.Extensions.Http;
using Polly;

public static class RelateHttpClient
{
    #region Properties
    public const string DisableTLS = "DisableTLS";
    public const string EnableTLS = "EnableTLS";
    #endregion

    #region Method
    public static void RegisterHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Http Client (transient life-time)
        services.AddHttpClient(DisableTLS, httpClient => { }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (m, c, ch, e) => true; // Ignore TLS

            return handler;
        }).AddPolicyHandler(GetRetryPolicy());

        services.AddHttpClient(EnableTLS, httpClient => { }).AddPolicyHandler(GetRetryPolicy());
    }

    #region Private work
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount = 3)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
    #endregion

    #endregion
}
