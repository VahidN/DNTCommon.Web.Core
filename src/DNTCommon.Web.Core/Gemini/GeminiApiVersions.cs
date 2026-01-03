namespace DNTCommon.Web.Core;

/// <summary>
///     A static class containing known Gemini API versions.
/// </summary>
public sealed class GeminiApiVersions
{
    /// <summary>
    ///     <c>v1</c> API version.
    /// </summary>
    public static readonly GeminiApiVersions V1 = "v1";

    /// <summary>
    ///     <c>v1alpha</c> API version.
    /// </summary>
    public static readonly GeminiApiVersions V1Alpha = "v1alpha";

    /// <summary>
    ///     <c>v1beta</c> API version.
    /// </summary>
    public static readonly GeminiApiVersions V1Beta = "v1beta";

    private GeminiApiVersions(string value) => Value = value;

    public string Value { get; }

    public static implicit operator GeminiApiVersions(string value) => new(value);

    public override string ToString() => Value;

    public static GeminiApiVersions ToGeminiApiVersions(string value) => new(value);
}
