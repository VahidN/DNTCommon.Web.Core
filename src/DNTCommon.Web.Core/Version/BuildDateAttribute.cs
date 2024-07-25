namespace DNTCommon.Web.Core;

/// <summary>
///     A version helper
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class BuildDateAttribute : Attribute
{
    /// <summary>
    ///     A version helper
    /// </summary>
    public BuildDateAttribute(string buildDateTime) => BuildDateTime = buildDateTime;

    /// <summary>
    ///     Returns the build date of the Assembly in the specified culture.
    /// </summary>
    public string BuildDateTime { get; }
}