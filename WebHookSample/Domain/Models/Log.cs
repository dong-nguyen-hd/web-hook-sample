namespace WebHookSample.Domain.Models;

using Resources.Enums;

public sealed class Log
{
    public string Id { get; set; } = new IdGenerator(0).CreateId().ToString();
    public string? Node { get; set; }
    public string? ClientIp { get; set; }
    public string? WebHookId { get; set; }
    public string? TraceId { get => traceId; set => traceId = value?.Replace(':', '_'); }
    private string? traceId;
    public LogType? LogType { get; set; }
    public Dictionary<string, string>? RequestHeaders { get; set; }
    public Dictionary<string, string>? RequestQueries { get; set; }
    public DateTime? RequestDatetimeUtc { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestQuery { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestScheme { get; set; }
    public string? RequestHost { get; set; }
    public string? RequestBody { get; set; }
    public string? RequestContentType { get; set; }
    public DateTime? ResponseDatetimeUtc { get; set; }
    public string? ResponseStatus { get; set; }
    public string? ResponseBody { get; set; }
    public string? ResponseContentType { get; set; }
    public Dictionary<string, string>? ResponseHeaders { get; set; }
    public int TotalTimeMs => ComputeTotalTimeMs();
    public bool HasException { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? ExceptionStackTrace { get; set; }

    #region Method
    private int ComputeTotalTimeMs()
    {
        if (ResponseDatetimeUtc == null || RequestDatetimeUtc == null)
            return 0;

        return Convert.ToInt32((ResponseDatetimeUtc.Value - RequestDatetimeUtc.Value).TotalMilliseconds);
    }
    #endregion
}