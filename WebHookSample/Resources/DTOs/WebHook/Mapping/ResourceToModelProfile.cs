using AutoMapper;
using WebHookSample.Resources.DTOs.WebHook.Request;

namespace WebHookSample.Resources.DTOs.WebHook.Mapping;

public sealed class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<CreateWebHookRequest, Models.WebHook>()
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}