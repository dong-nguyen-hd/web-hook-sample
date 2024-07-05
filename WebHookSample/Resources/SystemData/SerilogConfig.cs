namespace WebHookSample.Resources.SystemData;

public sealed class SerilogConfig
{
    public static string? PathLogFile { get; private set; }
    public static bool DisableConsoleLog { get; private set; }
}