using Polly;
using Polly.Retry;
using WebHookSample.Domain.Services;
using WebHookSample.Resources.Enums;
using HttpMethod = WebHookSample.Resources.Enums.HttpMethod;

namespace WebHookSample.Services;

public sealed class CustomHttpClient(
    IHttpClientFactory httpClientFactory,
    ILogService logService,
    IHttpContextAccessor httpContextAccessor) : ICustomHttpClient
{
    #region Properties

    private readonly HttpContext? _httpContext = httpContextAccessor?.HttpContext;

    #endregion

    #region Method

    public async Task<ProcessType> SendAsync(Models.WebHook request, CancellationToken cancellationToken)
    {
        var log = GetLog(request);

        try
        {
            // Set up client
            var client = GetHttpClient(request);
            SetHeader(client, request, log);

            // Request
            HttpRequestMessage httpRequest = new()
            {
                Method = MappingHttpMethod(request),
                RequestUri = request.Uri,
                Content = new StringContent(request.Payload ?? string.Empty, Encoding.UTF8, MimeType.JSON),
            };
            var response = await RetryRequestAsync(request, client, httpRequest, cancellationToken);
            SetLogResponse(log, response);

            // Process result
            if (response.IsSuccessStatusCode)
                return ProcessType.Success;

            return ProcessType.Fail;
        }
        catch (Exception ex)
        {
            SetLogException(log, ex);
            return ProcessType.Fail;
        }
        finally
        {
            logService.Write(log);
        }
    }

    #region Private work

    /// <summary>
    /// Role: get instance of Httpclient
    /// </summary>
    /// <param name="webHook"></param>
    /// <returns></returns>
    private HttpClient GetHttpClient(Models.WebHook webHook)
    {
        if (webHook.EnableVerifyTls)
            return httpClientFactory.CreateClient(RelateHttpClient.EnableTLS);

        return httpClientFactory.CreateClient(RelateHttpClient.DisableTLS);
    }

    /// <summary>
    /// Role: mapping myHttpMethod to System.Net.Http.HttpMethod
    /// </summary>
    /// <param name="webHook"></param>
    /// <returns></returns>
    private static System.Net.Http.HttpMethod MappingHttpMethod(Models.WebHook webHook)
    {
        switch (webHook.HttpMethod)
        {
            case HttpMethod.DELETE:
                return System.Net.Http.HttpMethod.Delete;
            case HttpMethod.GET:
                return System.Net.Http.HttpMethod.Get;
            case HttpMethod.POST:
                return System.Net.Http.HttpMethod.Post;
            case HttpMethod.PUT:
                return System.Net.Http.HttpMethod.Put;
            default:
                return System.Net.Http.HttpMethod.Get;
        }
    }

    /// <summary>
    /// Role: retry request mechanism
    /// </summary>
    /// <param name="webHook"></param>
    /// <param name="httpClient"></param>
    /// <param name="httpRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> RetryRequestAsync(Models.WebHook webHook, HttpClient httpClient, HttpRequestMessage httpRequest, CancellationToken cancellationToken)
    {
        var maxRetryAttempts = webHook.NumberRetry;
        if (maxRetryAttempts != 0)
        {
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);

            var pipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                {
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<Exception>()
                        .HandleResult(static result => !result.IsSuccessStatusCode),
                    Delay = pauseBetweenFailures,
                    MaxRetryAttempts = maxRetryAttempts,
                    BackoffType = DelayBackoffType.Constant
                })
                .Build();

            return await pipeline.ExecuteAsync(async token =>
                await httpClient.SendAsync(new HttpRequestMessage()
                {
                    Method = httpRequest.Method,
                    RequestUri = httpRequest.RequestUri,
                    Content = httpRequest.Content
                }, cancellationToken), cancellationToken);
        }

        return await httpClient.SendAsync(httpRequest, cancellationToken);
    }

    /// <summary>
    /// Role: set header for request
    /// </summary>
    /// <param name="client"></param>
    /// <param name="webHook"></param>
    /// <param name="log"></param>
    private void SetHeader(HttpClient client, Models.WebHook webHook, Models.Log log)
    {
        // Set other header
        if (webHook.Headers is not null and { Count: > 0 })
        {
            Dictionary<string, string> headers = new();

            foreach (var header in webHook.Headers)
            {
                if (string.IsNullOrEmpty(header.Key))
                    continue;

                client.DefaultRequestHeaders.Add(header.Key, header.Value ?? string.Empty);
                headers.TryAdd(header.Key, header.Value ?? string.Empty);
            }

            log.RequestHeaders = headers;
        }
    }

    /// <summary>
    /// Role: create instance of Models.Log
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private Models.Log GetLog(Models.WebHook request)
    {
        Models.Log temp = new()
        {
            Node = SystemInformation.Node,
            TraceId = _httpContext != null ? _httpContext.TraceIdentifier : "<none>",
            WebHookId = request.Id,
            LogType = LogType.ThirdPartyLog,
            RequestMethod = nameof(request.HttpMethod),
            RequestDatetimeUtc = DateTime.UtcNow,
            RequestPath = request.Uri.AbsolutePath,
            RequestQuery = request.Uri.Query,
            RequestHost = request.Uri.Host,
            RequestScheme = request.Uri.Scheme,
            RequestQueries = request.Uri.Query.FormatQueries()
        };

        return temp;
    }

    /// <summary>
    /// Role: assign addtional data to log
    /// </summary>
    /// <param name="log"></param>
    /// <param name="response"></param>
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

    /// <summary>
    /// Role: assign exception to log
    /// </summary>
    /// <param name="log"></param>
    /// <param name="ex"></param>
    private void SetLogException(Models.Log log, Exception ex)
    {
        log.HasException = true;
        log.ExceptionMessage = ex.Message;
        log.ExceptionStackTrace = ex.StackTrace;
    }

    #endregion

    #endregion
}