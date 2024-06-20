namespace WebHookSample.Extensions;

using System.Text.RegularExpressions;

public static class RelateText
{
    /// <summary>
    /// Chức năng: xoá các kí tự khoảng trắng bị lặp lại (2 kí tự space -> 1 kí tự space)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveSpaceCharacter(this string? text) =>
        string.IsNullOrEmpty(text) ? string.Empty : Regex.Replace(text.Trim(), @"\s{2,}", " ");

    /// <summary>
    /// Chức năng: xoá kí tự khoảng trắng bị lặp và viết thường tất cả
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToLowerAndRemoveSpace(this string? text) =>
        RemoveSpaceCharacter(text).ToLower();

    /// <summary>
    /// Chức năng: loại bỏ toàn bộ kí tự khoảng trắng khỏi chuỗi
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveAllSpaceChar(this string? text) =>
       string.IsNullOrEmpty(text) ? string.Empty : Regex.Replace(text.Trim(), @"\s+", "");

    #region MySerialize
    private static JsonSerializerOptions _opt = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Chức năng: sử dụng Serialize với CamelCase cho đồng bộ toàn hệ thống
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string MySerialize<T>(this T source)
    {
        if (source is null)
            return string.Empty;

        return JsonSerializer.Serialize(source, _opt);
    }
    #endregion
}
