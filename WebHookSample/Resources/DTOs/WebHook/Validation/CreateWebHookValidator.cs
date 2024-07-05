using FluentValidation;
using WebHookSample.Resources.DTOs.WebHook.Request;

namespace WebHookSample.Resources.DTOs.WebHook.Validation;

public class CreateWebHookValidator : AbstractValidator<CreateWebHookRequest>
{
    public CreateWebHookValidator()
    {
        RuleFor(x => x.Uri).NotEmpty().NotNull();
    }
}