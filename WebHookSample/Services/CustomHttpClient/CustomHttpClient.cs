using Polly;
using Polly.Retry;
using WebHookSample.Domain.Services;
using WebHookSample.Resources.Enums;

namespace WebHookSample.Services.CustomHttpClient;

public sealed partial class CustomHttpClient(
    IHttpClientFactory httpClientFactory,
    ILogService logService,
    IHttpContextAccessor httpContextAccessor) : ICustomHttpClient
{
    #region Properties

    private readonly HttpContext? _httpContext = httpContextAccessor?.HttpContext;

    #endregion

    #region Method

    private async Task RetryRequestAsync(Models.WebHook webHook, Task<HttpResponseMessage> task, CancellationToken cancellationToken)
    {
        var maxRetryAttempts = webHook.NumberRetry;

        if (maxRetryAttempts == 0)
        {
            await task;
        }
        else
        {
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);

            var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                {
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<Exception>()
                        .HandleResult(static result =>
                        {
                            Serilog.Log.Information(result.ToString());
                            return !result.IsSuccessStatusCode;
                        }),
                    Delay = pauseBetweenFailures,
                    MaxRetryAttempts = maxRetryAttempts,
                    BackoffType = DelayBackoffType.Constant
                })
                .Build();

            await pipeline.ExecuteAsync(async token => await task, cancellationToken);
        }
    }

    private void SetHeader(HttpClient client, Models.WebHook webHook, Models.Log log)
    {
        Dictionary<string, string> headers = new();

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

    private void SetLogResponse(Models.Log log, HttpResponseMessage response)
    {
        log.ResponseDatetimeUtc = DateTime.UtcNow;
        log.ResponseStatus = response.StatusCode.ToString("D");
        log.ResponseContentType = response?.Content?.Headers?.ContentType?.MediaType;
        log.ResponseHeaders = GetHeader();

        Dictionary<string, string> GetHeader()
        {
            Dictionary<string, string> temp = new();

            foreach (var item in response?.Headers?.ToDictionary())
                temp.TryAdd(item.Key, string.Join(';', item.Value?.ToArray()));

            return temp;
        }
    }

    #endregion
}