using AutoMapper;
using WebHookSample.Domain.Context;
using WebHookSample.Domain.Services;
using WebHookSample.Resources.DTOs.WebHook.Request;
using WebHookSample.Resources.DTOs.WebHook.Response;

namespace WebHookSample.Services;

public sealed class WebHookService(IMapper mapper, CoreContext context) : BaseService(mapper, context), IWebHookService
{
    #region Method
    public async Task<WebHookResponse> CreateAsync(CreateWebHookRequest request)
    {
        throw new NotImplementedException();
    }
    #endregion
}