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
    public string? Message { get => _message; init => _message = GetMessage(value); }
    private string? _message;
    #endregion

    #region Constructor
    public BaseResult() { }

    public BaseResult(CodeMessage codeMessage, string? message = "") 
    {
        this.Data = default(T);
        this.CodeMessage = codeMessage;
        this.Message = message ?? string.Empty;
    }
    #endregion

    #region Method
    private string GetMessage(string? input)
    {
        if(string.IsNullOrEmpty(input))
            return ResponseMessage.Values.TryGetValue(StatusCode ?? string.Empty, out var value) ? value : string.Empty;

        return input.RemoveSpaceCharacter();
    }
    #endregion
}