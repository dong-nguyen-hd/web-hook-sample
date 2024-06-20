namespace WebHookSample.Resources.Results;

public class BaseResult<T>
{
    #region Properties
    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonIgnore]
    [JsonPropertyName("status")]
    public CodeMessage CodeMessage { get; init; }

    [JsonPropertyName("statusCode")]
    public string? StatusCode { get => CodeMessage.GetElementNameCodeMessage(); }

    [JsonPropertyName("message")]
    public string? Message { get; init; }
    #endregion

    #region Constructor
    public BaseResult() { }

    public BaseResult(CodeMessage codeMessage, string message = "") 
    {
        this.Data = default(T);
        this.CodeMessage = codeMessage;
        this.Message = message;
    }
    #endregion
}
