namespace WebHookSample.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Chức năng: lấy ra tên của phần tử CodeMessage enum
    /// </summary>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    public static string GetElementNameCodeMessage(this CodeMessage statusCode) =>
        Enum.GetName(typeof(CodeMessage), statusCode)?.TrimStart('_');
}