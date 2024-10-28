namespace WebHookSample.Services;

using AutoMapper;
using WebHookSample.Domain.Context;
using WebHookSample.Domain.Services;

public abstract class BaseService : IBaseService
{
    protected virtual BaseResult<T> GetBaseResult<T>(CodeMessage codeMessage, T? data = default, string message = "")
    {
        return new BaseResult<T>()
        {
            Data = data,
            CodeMessage = codeMessage,
            Message = message
        };
    }
}