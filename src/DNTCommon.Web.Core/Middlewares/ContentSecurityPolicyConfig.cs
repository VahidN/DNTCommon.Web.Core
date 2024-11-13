namespace DNTCommon.Web.Core;

/// <summary>
///     CSP config
/// </summary>
public class ContentSecurityPolicyConfig
{
    /// <summary>
    ///     CSP options. Each option should be specified in one line.
    /// </summary>
    public IList<string> Options { get; } = [];
}