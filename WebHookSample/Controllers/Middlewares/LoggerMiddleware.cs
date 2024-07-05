namespace WebHookSample.Controllers.Middlewares;

using WebHookSample.Domain.Services;
using Microsoft.AspNetCore.Diagnostics;

#region Log Creator
public interface ILogModelCreator
{
    Models.Log LogModel { get; }
    void Logging();
}

public sealed class LogModelCreator(ILogService logService) : ILogModelCreator
{
    public Models.Log LogModel { get; private set; } = new();

    public void Logging() =>
        logService.Write(LogModel);
}
#endregion

#region Middleware
public sealed class LoggerMiddleware(RequestDelegate next)
{
    #region Method
    public async Task InvokeAsync(HttpContext httpContext, ILogModelCreator logCreator)
    {
        var log = logCreator.LogModel;

        log.RequestDatetimeUtc = DateTime.UtcNow;
        log.LogType = Resources.Enums.LogType.Log;
        HttpRequest request = httpContext.Request;

        // Log
        log.TraceId = httpContext.TraceIdentifier;
        log.ClientIp = request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        log.Node = SystemInformation.Node;

        // Request
        log.RequestMethod = request.Method;
        log.RequestPath = request.Path;
        log.RequestQuery = request.QueryString.ToString();
        log.RequestQueries = FormatQueries(request.QueryString.ToString());
        log.RequestHeaders = FormatHeaders(request.Headers);
        log.RequestBody = await ReadBodyFromRequest(request);
        log.RequestScheme = request.Scheme;
        log.RequestHost = request.Host.ToString();
        log.RequestContentType = request.ContentType ?? string.Empty;

        // Change HttpResponseStream (read-only) by MemoryStream (allow write)
        HttpResponse response = httpContext.Response;
        var originalResponseBody = response.Body;
        using var newResponseBody = new MemoryStream();
        response.Body = newResponseBody;

        // Call the next middleware in the pipeline
        try
        {
            await next(httpContext);
        }
        catch (Exception exception)
        {
            // Exception: but was not managed at app.UseExceptionHandler() or by any middleware
            LogError(log, exception);
        }

        newResponseBody.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await new StreamReader(response.Body).ReadToEndAsync();

        newResponseBody.Seek(0, SeekOrigin.Begin);
        await newResponseBody.CopyToAsync(originalResponseBody);

        // Response
        log.ResponseContentType = response.ContentType;
        log.ResponseStatus = response.StatusCode.ToString();
        log.ResponseHeaders = FormatHeaders(response.Headers);
        log.ResponseBody = responseBodyText;
        log.ResponseDatetimeUtc = DateTime.UtcNow;

        // Exception: but was managed at app.UseExceptionHandler() or by any middleware
        var contextFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (contextFeature != null && contextFeature.Error != null)
        {
            LogError(log, contextFeature.Error);
        }

        logCreator.Logging();
    }

    #region Private work
    private void LogError(Models.Log log, Exception exception)
    {
        Serilog.Log.Error(exception, $"LogId ({log.Id}): {exception.Message}");

        log.ExceptionMessage = exception.Message;
        log.ExceptionStackTrace = exception.StackTrace;
    }

    private Dictionary<string, string> FormatHeaders(IHeaderDictionary headers)
    {
        Dictionary<string, string> pairs = new();
        foreach (var header in headers)
            pairs.TryAdd(header.Key, header.Value);

        return pairs;
    }

    private Dictionary<string, string> FormatQueries(string queryString)
    {
        Dictionary<string, string> pairs = new();
        string key, value;
        foreach (var query in queryString.TrimStart('?').Split('&'))
        {
            var items = query.Split('=');
            key = items.Count() >= 1 ? items[0] : string.Empty;
            value = items.Count() >= 2 ? items[1] : string.Empty;
            if (!string.IsNullOrEmpty(key))
                pairs.TryAdd(key, value);
        }

        return pairs;
    }

    private async Task<string> ReadBodyFromRequest(HttpRequest request)
    {
        // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).
        request.EnableBuffering();
        using var streamReader = new StreamReader(request.Body, leaveOpen: true);
        var requestBody = await streamReader.ReadToEndAsync();

        // Reset the request's body stream position for next middleware in the pipeline.
        request.Body.Position = 0;

        return requestBody;
    }
    #endregion

    #endregion
}

#endregion