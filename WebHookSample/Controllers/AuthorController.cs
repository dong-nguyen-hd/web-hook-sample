using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebHookSample.Controllers.Config;
using WebHookSample.Resources.DTOs.Ready.Response;

namespace WebHookSample.Controllers;

public sealed class AuthorController(IMapper mapper) : ParentController(mapper)
{
    #region Action

    [HttpGet("ready")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<ReadyResponse>), 200)]
    [SwaggerOperation(summary: "Ready")]
    public IActionResult Ready()
    {
        var result = GetBaseResult(CodeMessage._99, new ReadyResponse());

        return Ok(result);
    }

    #endregion
}