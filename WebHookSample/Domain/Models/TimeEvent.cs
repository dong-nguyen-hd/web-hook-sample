namespace WebHookSample.Domain.Models;

using WebHookSample.Resources.Enums;

public sealed class TimeEvent
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public int Id { get; set; }
    public ProcessType ProcessType { get; set; }
    public DateTime TimeStampUtc { get; set; }
    public WebHook WebHook { get; set; }
}
