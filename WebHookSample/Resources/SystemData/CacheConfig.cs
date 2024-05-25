namespace WebHookSample.Resources.SystemData;

public sealed class CacheConfig
{
    public static string? RedisUri { get; set; }
    public static string? RedisInstanceName { get; set; }
    public static bool UseMemoryCache { get; set; }
}
