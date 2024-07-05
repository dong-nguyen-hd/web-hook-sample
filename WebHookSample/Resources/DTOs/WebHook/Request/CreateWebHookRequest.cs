using HttpMethod = WebHookSample.Resources.Enums.HttpMethod;

namespace WebHookSample.Resources.DTOs.WebHook.Request;

public sealed class CreateWebHookRequest
{
    public string Uri { get; set; }
    public string? ContentType { get; set; }
    [SensitiveData] public string? Payload { get; set; }
    public HttpMethod HttpMethod { get; set; }
    public int NumberRetry { get; set; }
    public bool EnableVerifyTls { get; set; }
    public DateTime TriggerDatetimeUtc { get; set; }
}