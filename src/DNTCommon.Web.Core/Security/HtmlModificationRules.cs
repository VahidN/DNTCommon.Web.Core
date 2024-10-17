namespace DNTCommon.Web.Core;

/// <summary>
///     How to change the given HTML
/// </summary>
public class HtmlModificationRules
{
    /// <summary>
    ///     The current Request's root address
    /// </summary>
    public Uri? HostUri { set; get; }

    /// <summary>
    ///     Convert all the HTML Ps to DIVs
    /// </summary>
    public bool ConvertPToDiv { set; get; }

    /// <summary>
    ///     Removes 'rel' = 'noopener noreferrer' and 'target' = '_blank' from internal links
    /// </summary>
    public bool RemoveRelAndTargetFromInternalUrls { set; get; }

    /// <summary>
    ///     Default style of the pre and code elemets
    /// </summary>
    public string PreCodeStyles { set; get; } =
        "white-space: pre-wrap; overflow: auto; word-break: break-word; text-align: left; margin-top: 0.3rem; margin-bottom: 0.3rem;";
}