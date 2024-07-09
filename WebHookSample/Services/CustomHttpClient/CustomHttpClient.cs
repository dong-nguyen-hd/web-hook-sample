using WebHookSample.Domain.Services;
using WebHookSample.Resources.Enums;

namespace WebHookSample.Services.CustomHttpClient;

public sealed partial class CustomHttpClient(
    IHttpClientFactory httpClientFactory,
    ILogService logService,
    IHttpContextAccessor httpContextAccessor) : ICustomHttpClient
{
    #region Properties

    private static readonly string contentType = "Content-Type";
    private readonly HttpContext? _httpContext = httpContextAccessor?.HttpContext;

    #endregion

    #region Method

    private void SetHeader(HttpClient client, Models.WebHook webHook, Models.Log log)
    {
        Dictionary<string, string> headers = new();

        // Set content type
        if (!string.IsNullOrEmpty(webHook.ContentType))
        {
            client.DefaultRequestHeaders.Add(contentType, webHook.ContentType);
            headers.TryAdd(contentType, webHook.ContentType);
        }

        // Set other header
        if (webHook.Headers is not null and { Count: > 0 })
            foreach (var header in webHook.Headers)
            {
                if (string.IsNullOrEmpty(header.Key))
                    continue;

                client.DefaultRequestHeaders.Add(header.Key, header.Value ?? string.Empty);
                headers.TryAdd(header.Key, header.Value ?? string.Empty);
            }

        log.RequestHeaders = headers;
    }

    /// <summary>
    /// Role: create instance of Models.Log
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private Models.Log GetLog(Models.WebHook request)
    {
        Uri uri = new Uri(request.Uri);

        Models.Log temp = new()
        {
            Node = SystemInformation.Node,
            TraceId = _httpContext != null ? _httpContext.TraceIdentifier : "<none>",
            WebHookId = request.Id,
            LogType = LogType.ThirdPartyLog,
            RequestMethod = request.HttpMethod.ToString(),
            RequestDatetimeUtc = DateTime.UtcNow,
            RequestPath = uri.AbsolutePath,
            RequestQuery = uri.Query,
            RequestHost = uri.Host,
            RequestScheme = uri.Scheme,
            RequestQueries = uri.Query.FormatQueries()
        };

        return temp;
    }

    #endregion
}