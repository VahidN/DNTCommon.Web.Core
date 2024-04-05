using DNTPersianUtils.Core;

namespace DNTCommon.Web.Core;

/// <summary>
///     A CSS builder helper
/// </summary>
public static class PersianExtensions 
{
    /// <summary>
    ///     Direction of the text based on its characters. It can be ltr or rtl.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string? GetDir(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return text.ContainsFarsi() ? "rtl" : "ltr";
    }
}