namespace WebHookSample.Resources.SystemData;

public sealed class ResponseMessage
{
    #region Property
    public static Dictionary<string, string>? Values { get; set; }
    #endregion
}

public enum CodeMessage
{
    _99,
    _100,
}