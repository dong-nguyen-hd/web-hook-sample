using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace WebHookSample.Controllers;

public abstract class ParentController : ControllerBase
{
    #region Properties
    protected readonly IMapper Mapper;
    #endregion

    #region Constructor
    public ParentController(IMapper mapper)
    {
        this.Mapper = mapper;
    }
    #endregion

    #region Method
    protected virtual BaseResult<Inner> GetBaseResult<Inner>(CodeMessage codeMessage, Inner? data = default, string message = "")
    {
        return new BaseResult<Inner>()
        {
            Data = data,
            CodeMessage = codeMessage,
            Message = message
        };
    }
    #endregion
}