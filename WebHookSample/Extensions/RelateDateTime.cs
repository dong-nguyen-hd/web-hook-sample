using TimeZoneConverter;

namespace WebHookSample.Extensions;

public static class RelateDateTime
{
    /// <summary>
    /// Role: convert Datetime to (iso-8601) format "yyyy-MM-ddTHH:mm:sssZ"
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ConvertToSystemFormat(this DateTime data)
        => data.ToString("yyyy-MM-ddThh:mm:ss.fffZ");

    /// <summary>
    /// Role: Convert UTC to +7:00
    /// </summary>
    /// <param name="utc"></param>
    /// <returns></returns>
    public static DateTime ConvertUtcToVietnamTz(this DateTime utc)
    {
        var windowsTz = "SE Asia Standard Time"; // +7:00

        TimeZoneInfo localTz = TZConvert.GetTimeZoneInfo(windowsTz);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, localTz);
    }

    /// <summary>
    /// Role: Convert +7:00 to UTC
    /// </summary>
    /// <param name="localTime"></param>
    /// <returns></returns>
    public static DateTime ConvertVietnamTzToUtc(this DateTime localTime)
    {
        var windowsTz = "SE Asia Standard Time"; // +7:00

        TimeZoneInfo localTz = TZConvert.GetTimeZoneInfo(windowsTz);
        return TimeZoneInfo.ConvertTimeToUtc(localTime, localTz);
    }
}