using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebHookSample.Controllers.Middlewares;

namespace WebHookSample.Controllers.Filters;

[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
public sealed class LoggerActionFilter : Attribute, IActionFilter
{
    private Models.Log GetLogModel(HttpContext context)
    {
        return context.RequestServices.GetService<ILogModelCreator>().LogModel;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var model = GetLogModel(context.HttpContext);

        Type? requestType = null;
        foreach (var parameterDescriptor in context.ActionDescriptor.Parameters)
        {
            var bindingSource = parameterDescriptor?.BindingInfo?.BindingSource;

            if (bindingSource == BindingSource.Body)
            {
                requestType = parameterDescriptor.ParameterType;
                break;
            }
        }

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null || argument.GetType() != requestType)
                continue;

            model.RequestBody = argument.MaskSensitiveData();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var model = GetLogModel(context.HttpContext);

        if (context.Result != null && context.Result.GetType().IsSubclassOf(typeof(ObjectResult)))
        {
            var res = ((ObjectResult)context.Result).Value;

            if (res != null)
                model.ResponseBody = res.MaskSensitiveData();
        }
    }
}