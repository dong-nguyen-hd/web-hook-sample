namespace WebHookSample.Extensions.AddConfig;

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
        });

        services.AddHttpClient(EnableTLS, httpClient => { });
    }

    #endregion
}