namespace WebHookSample.Controllers.Middlewares;

using Microsoft.AspNetCore.Http;

public sealed class ErrorHandlerMiddleware(RequestDelegate next)
{
    #region Method
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = MimeType.JSON;
            BaseResult<object> result;

            // Using switch for custom exception
            switch (error)
            {
                // Add custom exception code below!
                case TaskCanceledException ex:
                    result = new(CodeMessage._101);
                    break;
                default:
                    // unhandled error
                    result = new(CodeMessage._100);
                    break;
            }

            await response.WriteAsync(result.MySerialize());

            throw;
        }
    }
    #endregion
}
