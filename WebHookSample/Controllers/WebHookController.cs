using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebHookSample.Controllers.Config;
using WebHookSample.Domain.Services;
using WebHookSample.Resources.DTOs.WebHook.Request;
using WebHookSample.Resources.DTOs.WebHook.Response;

namespace WebHookSample.Controllers;

[Route("api/v1/web-hook")]
public sealed partial class WebHookController(IWebHookService webHookService, IMapper mapper) : ParentController(mapper)
{
    #region Action

    [HttpPost("create")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<WebHookResponse>), 200)]
    [SwaggerOperation(summary: "Create web hook")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateWebHookRequest request, CancellationToken token)
    {
        var result = await webHookService.CreateAsync(request, token);
        return Ok(result);
    }

    #endregion
}