namespace WebHookSample.Services;

using AutoMapper;
using Microsoft.Extensions.Options;
using WebHookSample.Domain.Services;

public abstract class BaseService : IBaseService
{
    #region Properties
    protected readonly IMapper Mapper;
    private readonly ResponseMessage _responseMessage;
    #endregion

    #region Constructor
    public BaseService(IMapper mapper,
        IOptionsMonitor<ResponseMessage> responseMessage)
    {
        this.Mapper = mapper;
        this._responseMessage = responseMessage.CurrentValue;
    }
    #endregion

    #region Method
    protected virtual BaseResult<Inner> GetBaseResult<Inner>(CodeMessage codeMessage, Inner? data = default, string message = "")
    {
        string code = codeMessage.GetElementNameCodeMessage().RemoveSpaceCharacter();
        string tempMessage = string.IsNullOrEmpty(message) ? ResponseMessage.Values[code].RemoveSpaceCharacter() : message.RemoveSpaceCharacter();

        return new BaseResult<Inner>()
        {
            Data = data,
            CodeMessage = codeMessage,
            Message = tempMessage
        };
    }
    #endregion
}
