namespace WebHookSample.Controllers.Config;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public static class InvalidResponseFactory
{
    public static IActionResult ProduceErrorResponse(ActionContext context)
    {
        var error = context.ModelState.GetErrorMessages();
        var response = new BaseResult<object>(CodeMessage._100, error);

        return new BadRequestObjectResult(response);
    }

    public static string? GetErrorMessages(this ModelStateDictionary dictionary)
        => dictionary.SelectMany(m => m.Value.Errors).Select(m => m.ErrorMessage).FirstOrDefault();
}
