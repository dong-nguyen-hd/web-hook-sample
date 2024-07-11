using Microsoft.OpenApi.Models;

namespace WebHookSample.Extensions.AddConfig;

public static class RelateSwagger
{
    public static void AddCustomizeSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.ToString());
            c.EnableAnnotations();
            c.SwaggerDoc($"v1", new OpenApiInfo { Title = SystemInformation.ApplicationName, Version = $"v{SystemInformation.Version}" });
        });
    }
}