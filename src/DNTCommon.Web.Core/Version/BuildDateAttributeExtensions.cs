namespace DNTCommon.Web.Core;

/// <summary>
///     A version helper
/// </summary>
public static class BuildDateAttributeExtensions
{
    /// <summary>
    ///     Returns the build date of the Assembly in the specified culture.
    /// </summary>
    public static string? GetBuildDateTime(this Assembly? assembly) =>
        assembly?.GetCustomAttribute<BuildDateAttribute>()?.BuildDateTime ??
        assembly?.GetName().Version?.ToString();
}