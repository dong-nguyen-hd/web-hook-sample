namespace WebHookSample.Extensions.AddConfig;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Formatting.Json;

public static class RelateLogConfig
{
    public static Serilog.ILogger LogWithContext(this string context) =>
        Log.ForContext("SourceContext", context);

    public static void AddLog(this IServiceCollection services, IConfiguration configuration)
    {
        var logCfg = new LoggerConfiguration();

        logCfg.MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProperty("ApplicationName", SystemInformation.ApplicationName);

        // Debug env
        if (SystemGlobal.IsDebug || !SerilogConfig.DisableConsoleLog)
        {
            logCfg.WriteTo.Console();
        }
        else // Production env
        {
            logCfg.WriteTo.Logger(lc =>
                lc.Filter.ByIncludingOnly(Matching.WithProperty<string>("SourceContext", p => p == "Microsoft.Hosting.Lifetime"))
                .WriteTo.Console());
        }

        logCfg.WriteTo.File(new JsonFormatter(), SerilogConfig.PathLogFile ?? "./logs/.json", rollingInterval: RollingInterval.Day);

        Log.Logger = logCfg.CreateLogger();
    }
}
