namespace WebHookSample.Resources.DTOs.Ready.Response;

public sealed class ReadyResponse
{
    public DateTime UtcNow { get; set; } = DateTime.UtcNow;
    public string? ProjectName { get; set; } = "Web Hook Service";
    public string? Author { get; set; } = "DongND";
    public string? Email { get; set; } = "dong.nguyen.hdkt@gmail.com";
    public string? Github { get; set; } = "https://github.com/dong-nguyen-hd";
}