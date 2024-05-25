namespace WebHookSample.Extensions.AddConfig;

using Microsoft.VisualBasic;

public static class RelateSystemData
{
    /// <summary>
    /// Chức năng: lấy dữ liệu config cho hệ thống <br/>
    /// Lưu ý: các biến dữ liệu hệ thống chỉ được phép đọc, không được thay đổi giá trị
    /// </summary>
    /// <param name="configuration"></param>
    public static void GetSystemData(this IServiceCollection services, IConfiguration configuration)
    {
        // Gán giá trị cho Global
        SystemGlobal.PostgresqlConnectionString = configuration.GetConnectionString("PostgreSQL");

        // Gán giá trị cho phần CO6
        configuration.GetSection(nameof(SystemInformation)).Get<SystemInformation>();

        // Gán giá trị cho phần CacheConfig
        configuration.GetSection(nameof(CacheConfig)).Get<CacheConfig>();

        // Gán giá trị cho phần SerilogConfig
        configuration.GetSection(nameof(SerilogConfig)).Get<SerilogConfig>();

        // Mapping data from response-message.json
        services.Configure<ResponseMessage>(configuration.GetSection(nameof(ResponseMessage)));
    }
}
