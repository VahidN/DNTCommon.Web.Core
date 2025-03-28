using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     SkiaSharp's font provider
/// </summary>
public static class SKTypefaceProvider
{
    private static readonly ConcurrentDictionaryLocked<string, SKTypeface> FontsTypeface =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     SkiaSharp's font provider
    /// </summary>
    public static SKTypeface GetFont(this string? fontName, SKFontStyle fontStyle, string? customFontPath = null)
    {
        if (customFontPath.IsEmpty() && !fontName.IsEmpty())
        {
            return FontsTypeface.LockedGetOrAdd(fontName, key => SKTypeface.FromFamilyName(key, fontStyle));
        }

        if (!customFontPath.IsEmpty())
        {
            return FontsTypeface.LockedGetOrAdd(customFontPath, key =>
            {
                using var embeddedFont = File.OpenRead(key);

                return SKTypeface.FromStream(embeddedFont);
            });
        }

        throw new InvalidOperationException(message: "Please specify a font-name or a font-path.");
    }
}
