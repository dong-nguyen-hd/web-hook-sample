using WebHookSample.Resources.Enums;

namespace WebHookSample.Domain.Models;

public sealed class TimeEvent
{
    public string Id { get; set; } = new IdGenerator(0).CreateId().ToString();
    public ProcessType ProcessType { get; set; }
    public DateTime TimeStampUtc { get; set; }
    public string WebHookId { get; set; }
    public WebHook WebHook { get; set; }
}