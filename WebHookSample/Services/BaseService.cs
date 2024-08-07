﻿namespace WebHookSample.Services;

using AutoMapper;
using WebHookSample.Domain.Context;
using WebHookSample.Domain.Services;

public abstract class BaseService : IBaseService
{
    #region Properties

    protected readonly IMapper Mapper;
    protected readonly CoreContext Context;

    #endregion

    #region Constructor

    public BaseService(IMapper mapper, CoreContext context)
    {
        this.Mapper = mapper;
        this.Context = context;
    }

    #endregion

    #region Method

    protected virtual BaseResult<T> GetBaseResult<T>(CodeMessage codeMessage, T? data = default, string message = "")
    {
        return new BaseResult<T>()
        {
            Data = data,
            CodeMessage = codeMessage,
            Message = message
        };
    }

    #endregion
}