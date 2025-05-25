namespace DNTCommon.Web.Core;

/// <summary>
///     Validators Extensions
/// </summary>
public static class ValidatorsExtensions
{
    /// <summary>
    ///     Tries to parse a string into a Guid-value.
    /// </summary>
    public static bool IsValidGuid(this string? value)
        => !value.IsEmpty() && Guid.TryParse(value, CultureInfo.InvariantCulture, out _);
}
