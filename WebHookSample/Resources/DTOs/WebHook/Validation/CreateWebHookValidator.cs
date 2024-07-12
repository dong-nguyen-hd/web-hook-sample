using FluentValidation;
using WebHookSample.Resources.DTOs.WebHook.Request;
using HttpMethod = WebHookSample.Resources.Enums.HttpMethod;

namespace WebHookSample.Resources.DTOs.WebHook.Validation;

public class CreateWebHookValidator : AbstractValidator<CreateWebHookRequest>
{
    public CreateWebHookValidator()
    {
        RuleFor(x => x.Uri).NotEmpty().NotNull().Must(IsUri);
        RuleFor(x => x.Payload).Must(IsJson).When(x => !string.IsNullOrEmpty(x.Payload));
        RuleFor(x => x.Headers)
            .Must(x => x.TrueForAll(y => !string.IsNullOrEmpty(y.Key)))
            .When(x => x.Headers != null && x.Headers.Count > 0);
        RuleFor(x => x.HttpMethod).Must(x => Enum.IsDefined(typeof(HttpMethod), x));
        RuleFor(x => x.NumberRetry).Must(x => x >= 0 && x < 4);
    }

    #region Private work

    private static bool IsUri(Uri uri)
    {
        string raw = uri.ToString();

        if (!Uri.IsWellFormedUriString(raw, UriKind.Absolute))
            return false;
        if (!Uri.TryCreate(raw, UriKind.Absolute, out var temp))
            return false;
        return temp.Scheme == Uri.UriSchemeHttp || temp.Scheme == Uri.UriSchemeHttps;
    }

    private static bool IsJson(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return false;

        try
        {
            using var jsonDoc = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}