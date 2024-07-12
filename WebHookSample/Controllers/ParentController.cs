using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

    protected static IActionResult ProduceErrorResponse(ValidationResult validate, ModelStateDictionary modelState)
    {
        // Add to ModelState from FluentValidation
        foreach (var temp in validate.Errors)
            modelState.AddModelError(temp.PropertyName, temp.ErrorMessage);

        var error = GetErrorMessages(modelState);
        var response = new BaseResult<object>(CodeMessage._100, error);

        return new BadRequestObjectResult(response);

        string? GetErrorMessages(ModelStateDictionary dictionary)
            => dictionary.SelectMany(m => m.Value.Errors).Select(m => m.ErrorMessage).FirstOrDefault();
    }

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