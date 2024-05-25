namespace WebHookSample.Domain.Models;

public sealed class Header
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public int Id { get; set; }
    public string? Key { get; set; }
    public string? Value { get; set; }
    public WebHook WebHook { get; set; }
}
