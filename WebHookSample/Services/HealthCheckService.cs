using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using WebHookSample.Domain.Context;
using WebHookSample.Resources.DTOs.Ready.Response;

namespace WebHookSample.Services;

public sealed class HealthCheckService(CoreContext context) : BaseService, IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext healthCheckContext, CancellationToken cancellationToken = default)
    {
        bool postgresqlConnect = await context.Database.CanConnectAsync(cancellationToken);
        bool redis = (await ConnectionMultiplexer.ConnectAsync(CacheConfig.RedisUri ?? string.Empty)).IsConnected;

        var result = GetBaseResult(CodeMessage._99, new HealthCheckResponse()
        {
            Information = new(),
            Postgresql = postgresqlConnect,
            Redis = redis
        });

        return HealthCheckResult.Healthy(result.MySerialize());
    }
}