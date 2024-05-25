namespace WebHookSample.Domain.Models;

public sealed class WebHook
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public int Id { get; set; }
    public string Uri { get; set; }
    public string? ContentType { get; set; }
    public string? Payload { get; set; }
    public Resources.Enums.HttpMethod HttpMethod { get; set; }
    public int NumberRetry { get; set; }
    public bool IsProcess { get; set; }
    public bool EnableVerifyTls { get; set; }
    public DateTime CreatedDatetimeUtc { get; set; }
    public DateTime TriggerDatetimeUtc { get; set; }
    public HashSet<Header> Headers { get; set; }
    public HashSet<TimeEvent> TimeEvents { get; set; }
}
