using AutoMapper;
using WebHookSample.Resources.DTOs.TimeEvent.Response;

namespace WebHookSample.Resources.DTOs.TimeEvent.Mapping;

public sealed class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Models.TimeEvent, TimeEventResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}