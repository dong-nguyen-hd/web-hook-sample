using FluentValidation;
using WebHookSample.Controllers.Filters;
using WebHookSample.Controllers.Middlewares;
using WebHookSample.Domain.Services;
using WebHookSample.Resources.DTOs.WebHook.Mapping;
using WebHookSample.Resources.DTOs.WebHook.Validation;
using WebHookSample.Services;

namespace WebHookSample.Extensions.AddConfig;

public static class RelateServices
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICustomHttpClient, CustomHttpClient>();
        
        services.AddScoped<ILogModelCreator, LogModelCreator>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IWebHookService, WebHookService>();
        
        services.AddAutoMapper(typeof(ResourceToModelProfile));
        
        services.AddValidatorsFromAssemblyContaining<CreateWebHookValidator>();
        
        // Add Filter
        services.AddMvc(options =>
        {
            options.Filters.Add(new LoggerActionFilter());
        });
    }
}