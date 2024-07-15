namespace WebHookSample.Resources.SystemData;

public static class SystemGlobal
{
    #region PostgresqlConnectionString

    private static string? _postgresqlConnectionString;

    public static string? PostgresqlConnectionString
    {
        get => _postgresqlConnectionString;
        set => SetPostgresqlConnectionString(value);
    }

    private static void SetPostgresqlConnectionString(string? value)
    {
        if (!string.IsNullOrEmpty(_postgresqlConnectionString))
            throw new InvalidOperationException();

        if (string.IsNullOrEmpty(_postgresqlConnectionString) && !string.IsNullOrEmpty(value))
            _postgresqlConnectionString = value;
    }

    #endregion

    #region IsDebug

    private static bool? _isDebug;

    public static bool IsDebug
    {
        get => _isDebug ?? false;
        set => SetIsDebug(value);
    }

    private static void SetIsDebug(bool value)
    {
        if (_isDebug != null)
            throw new InvalidOperationException();

        _isDebug ??= value;
    }

    #endregion

    public const string Masked = "*** Masked ***";
}