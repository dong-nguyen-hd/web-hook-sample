using WebHookSample.Resources.Enums;

namespace WebHookSample.Resources.DTOs.TimeEvent.Response;

public sealed class TimeEventResponse
{
    public ProcessType ProcessType { get; set; }
    public DateTime TimeStampUtc { get; set; }
}