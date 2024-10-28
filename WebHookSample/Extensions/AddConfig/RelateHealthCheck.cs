using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthCheckService = WebHookSample.Services.HealthCheckService;

namespace WebHookSample.Extensions.AddConfig;

public static class RelateHealthCheck
{
    public static void AddHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks().AddCheck<HealthCheckService>("Third-Party-Check");
    }

    public static void MapHealthCheck(this WebApplication webApplication)
    {
        webApplication.MapHealthChecks("/health-check", new HealthCheckOptions
            {
                ResponseWriter = WriteResponse
            }).AllowAnonymous(); // TODO: Add an authentication method.
    }

    private static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        foreach (var healthReportEntry in healthReport.Entries)
            if (!string.IsNullOrEmpty(healthReportEntry.Value.Description))
                return context.Response.WriteAsync(healthReportEntry.Value.Description);

        return context.Response.WriteAsync("Unhandle exception!");
    }
}