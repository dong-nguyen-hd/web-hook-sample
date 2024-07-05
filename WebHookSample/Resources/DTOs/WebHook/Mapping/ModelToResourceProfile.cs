using AutoMapper;
using WebHookSample.Resources.DTOs.WebHook.Response;

namespace WebHookSample.Resources.DTOs.WebHook.Mapping;

public sealed class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Models.WebHook, WebHookResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}