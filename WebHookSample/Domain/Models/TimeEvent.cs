namespace WebHookSample.Domain.Models;

using Resources.Enums;

public sealed class TimeEvent
{
    public string Id { get; set; } = new IdGenerator(0).CreateId().ToString();
    public ProcessType ProcessType { get; set; }
    public DateTime TimeStampUtc { get; set; }
    public string WebHookId { get; set; }
    public WebHook WebHook { get; set; }
}