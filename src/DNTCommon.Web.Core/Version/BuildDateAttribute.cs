namespace DNTCommon.Web.Core;

/// <summary>
///     A version helper
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class BuildDateAttribute(string buildDateTime) : Attribute
{
    /// <summary>
    ///     Returns the build date of the Assembly in the specified culture.
    /// </summary>
    public string BuildDateTime { get; } = buildDateTime;
}