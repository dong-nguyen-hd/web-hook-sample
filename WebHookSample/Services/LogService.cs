namespace WebHookSample.Services;

using WebHookSample.Domain.Services;

public sealed class LogService : ILogService
{
    #region Method
    public void Write<T>(T log) where T : class
    {
        var context = Log.ForContext("SourceContext", typeof(LogService).Name);

        if (log == null)
            context.Error("Error log null");

        context.Information($"Logging {{@Model}}: {{@{typeof(T).Name}}}", typeof(T).Name, log);
    }
    #endregion
}