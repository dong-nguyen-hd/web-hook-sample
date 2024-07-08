using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using ILogger = Serilog.ILogger;

namespace WebHookSample.Extensions.AddConfig;

#region Redaction declare

public static class Taxonomy
{
    public static string TaxonomyName => typeof(Taxonomy).FullName!;

    public static DataClassification SensitiveData => new(TaxonomyName, nameof(SensitiveData));
}

public class SensitiveDataAttribute() : DataClassificationAttribute(Taxonomy.SensitiveData);

public class MyErasingRedactor : Redactor
{
    private const string ErasedValue = "*** Masked ***"; // Use this value for sensitive data

    public override int GetRedactedLength(ReadOnlySpan<char> input)
        => ErasedValue.Length;

    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        // The base class ensures destination has sufficient capacity
        ErasedValue.CopyTo(destination);
        return ErasedValue.Length;
    }
}

#endregion

public static class RelateLogConfig
{
    public static ILogger LogWithContext(this string context) =>
        Log.ForContext("SourceContext", context);

    public static void AddLog(this IServiceCollection services, IConfiguration configuration)
    {
        #region Config log-type

        var logCfg = new LoggerConfiguration();

        logCfg
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProperty("ApplicationName", SystemInformation.ApplicationName);

        if (SerilogConfig.EnableConsoleLog)
            logCfg.WriteTo.Console();
        else
        {
            // Log only important information
            logCfg.WriteTo.Logger(lc =>
                lc.Filter.ByIncludingOnly(Matching.WithProperty<string>("SourceContext", p => p == "Microsoft.Hosting.Lifetime"))
                    .WriteTo.Console());
        }

        if (SerilogConfig.EnableFileLog)
            logCfg.WriteTo.File(new JsonFormatter(), SerilogConfig.PathFileLog ?? "./logs/.json", rollingInterval: RollingInterval.Day);

        if (SerilogConfig.EnableElasticsearchLog)
        {
            logCfg.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(SerilogConfig.ElasticsearchUri))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                ModifyConnectionSettings = (settings) =>
                {
                    settings.ServerCertificateValidationCallback((o, certificate, arg3, arg4) => true);
                    settings.BasicAuthentication(SerilogConfig.ElasticsearchUsername, SerilogConfig.ElasticsearchUsername);
                    return settings;
                },
                IndexFormat = SerilogConfig.ElasticsearchIndexFormat
            });
        }

        Log.Logger = logCfg.CreateLogger();

        #endregion

        #region Config Redaction

        // services.AddRedaction(x => { x.SetRedactor<MyErasingRedactor>(new DataClassificationSet(Taxonomy.SensitiveData)); });
        //
        // services.AddSerilog((sp, loggerConfiguration) => { loggerConfiguration.Destructure.WithRedaction(sp.GetRequiredService<IRedactorProvider>()); });

        #endregion
    }
}