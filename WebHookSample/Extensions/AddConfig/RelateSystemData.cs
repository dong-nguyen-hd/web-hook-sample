namespace WebHookSample.Extensions.AddConfig;

public static class RelateSystemData
{
    /// <summary>
    /// Role: retrieve configuration data for the system
    /// </summary>
    /// <param name="configuration"></param>
    public static void GetSystemData(this IServiceCollection services, IConfiguration configuration)
    {
        SystemGlobal.PostgresqlConnectionString = configuration.GetConnectionString("PostgreSQL");

        configuration.GetSection(nameof(SystemInformation)).Get<SystemInformation>(x => x.BindNonPublicProperties = true);

        configuration.GetSection(nameof(CacheConfig)).Get<CacheConfig>(x => x.BindNonPublicProperties = true);

        configuration.GetSection(nameof(SerilogConfig)).Get<SerilogConfig>(x => x.BindNonPublicProperties = true);

        configuration.GetSection(nameof(ResponseMessage)).Get<ResponseMessage>(x => x.BindNonPublicProperties = true);
    }
}
