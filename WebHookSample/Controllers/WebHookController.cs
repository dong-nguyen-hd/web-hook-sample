using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebHookSample.Controllers.Config;
using WebHookSample.Domain.Services;
using WebHookSample.Resources.DTOs.WebHook.Request;
using WebHookSample.Resources.DTOs.WebHook.Response;

namespace WebHookSample.Controllers;

[Route("api/v1/web-hook")]
public sealed class WebHookController(IWebHookService webHookService, IMapper mapper) : ParentController(mapper)
{
    #region Action

    [HttpPost("create")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<WebHookResponse>), 200)]
    [SwaggerOperation(summary: "Create web hook")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateWebHookRequest request, [FromServices] IValidator<CreateWebHookRequest> validator, CancellationToken token, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await webHookService.CreateAsync(request, token);
        return GetBaseResult(200, result);
    }

    #endregion
}