using WebHookSample.Domain.Models.ToJson;
using WebHookSample.Resources.Attribute;
using HttpMethod = WebHookSample.Resources.Enums.HttpMethod;

namespace WebHookSample.Resources.DTOs.WebHook.Request;

public sealed class CreateWebHookRequest
{
    public Uri Uri { get; set; }
    [SensitiveData] public string? Payload { get; set; }
    [SensitiveData] public List<Header>? Headers { get; set; }
    public HttpMethod HttpMethod { get; set; }
    public int NumberRetry { get; set; }
    public bool EnableVerifyTls { get; set; }
    public DateTime TriggerDatetimeUtc { get; set; }
}