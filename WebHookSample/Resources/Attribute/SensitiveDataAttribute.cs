namespace WebHookSample.Resources.Attribute;

/// <summary>
/// Role: remove sensitve data in DTO
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class SensitiveDataAttribute() : System.Attribute
{
}