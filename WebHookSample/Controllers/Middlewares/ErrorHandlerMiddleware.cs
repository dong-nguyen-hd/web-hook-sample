using FluentValidation;

namespace WebHookSample.Controllers.Middlewares;

using Microsoft.AspNetCore.Http;

public sealed class ErrorHandlerMiddleware(RequestDelegate next)
{
    public const string ErrorHandlerMiddlewareContext = nameof(ErrorHandlerMiddleware);
    
    #region Method

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
            
            // Xử lí cho mã 404
            if (context.Response.StatusCode == 404)
            {
                var response = context.Response;
                response.ContentType = MimeType.JSON;
                await response.WriteAsync(new BaseResult<object>(CodeMessage._100).MySerialize());
            }
        }
        catch (Exception error)
        {
            ErrorHandlerMiddlewareContext.LogWithContext().Error(error, error.Message);
            
            var response = context.Response;
            response.ContentType = MimeType.JSON;
            BaseResult<object> result;

            // Using switch for custom exception
            switch (error)
            {
                // Add custom exception code below!
                case TaskCanceledException ex1:
                case OperationCanceledException ex2:
                    result = new(CodeMessage._101);
                    break;
                case ValidationException:
                    result = new(CodeMessage._100);
                    break;
                default:
                    // unhandled error
                    if (SystemGlobal.IsDebug)
                        result = new(CodeMessage._100, error.Message);
                    else
                        result = new(CodeMessage._100);
                    break;
            }

            await response.WriteAsync(result.MySerialize());

            throw;
        }
    }

    #endregion
}