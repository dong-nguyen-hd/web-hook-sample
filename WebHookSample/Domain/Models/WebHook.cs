using WebHookSample.Domain.Models.ToJson;
using WebHookSample.Resources.Enums;

namespace WebHookSample.Domain.Models;

public sealed class WebHook
{
    public string Id { get; set; } = new IdGenerator(0).CreateId().ToString();
    public Uri Uri { get; set; }
    public string? Payload { get; set; }
    public Resources.Enums.HttpMethod HttpMethod { get; set; }
    public int NumberRetry { get; set; }
    public bool IsDone { get; set; }
    public ExecutionLevel Level { get; set; }
    public bool EnableVerifyTls { get; set; }
    public DateTime CreatedDatetimeUtc { get; set; }
    public DateTime TriggerDatetimeUtc { get; set; }
    public List<Header> Headers { get; set; }
    public HashSet<TimeEvent> TimeEvents { get; set; }
}