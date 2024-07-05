using WebHookSample.Controllers.Middlewares;
using WebHookSample.Domain.Services;
using WebHookSample.Services;

namespace WebHookSample.Extensions.AddConfig;

public static class RelateServices
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ILogModelCreator, LogModelCreator>();
        services.AddScoped<ILogService, LogService>();
    }
}