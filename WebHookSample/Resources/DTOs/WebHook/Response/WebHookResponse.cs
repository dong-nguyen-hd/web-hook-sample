using WebHookSample.Resources.DTOs.TimeEvent.Response;

namespace WebHookSample.Resources.DTOs.WebHook.Response;

public sealed class WebHookResponse
{
    public string Id { get; set; }
    public DateTime CreatedDatetimeUtc { get; set; }
    public DateTime TriggerDatetimeUtc { get; set; }
    public List<TimeEventResponse> TimeEvents { get; set; }
}