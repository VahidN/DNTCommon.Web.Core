using SkiaSharp;

namespace DNTCommon.Web.Core;

/// <summary>
///     Provides a random color from the SKColors struct
/// </summary>
public static class RandomSKColorProvider
{
    /// <summary>
    ///     Returns SKColors
    /// </summary>
    public static IReadOnlyList<SKColor> SkColors { get; } =
    [
        .. typeof(SKColors).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => field.GetValue(obj: null))
            .Where(value => value is not null)
            .Cast<SKColor>()
    ];

    /// <summary>
    ///     Returns a random color from SKColors
    /// </summary>
    public static SKColor RandomSkColor => SkColors[RandomNumberGenerator.GetInt32(SkColors.Count)];
}
