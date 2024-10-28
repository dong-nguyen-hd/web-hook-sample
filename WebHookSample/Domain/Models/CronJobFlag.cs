namespace WebHookSample.Domain.Models;

public sealed class CronJobFlag
{
    public uint Version { get; set; }
    
    public string Id { get; set; } = RelateText.GenId();

    public string? Name { get; set; }
    
    public DateTime ExecuteDatetimeUtc { get; set; }
}