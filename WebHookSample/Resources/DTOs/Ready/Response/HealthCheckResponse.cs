
namespace WebHookSample.Resources.DTOs.Ready.Response;

public sealed class HealthCheckResponse
{
    public ReadyResponse? Information { get; set; }
    public bool Postgresql { get; set; }
    public bool Redis { get; set; }
}