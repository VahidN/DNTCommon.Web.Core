using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     https://developer.chrome.com/blog/private-prefetch-proxy/
/// </summary>
public class PrefetchProxyTrafficAdvice
{
    /// <summary>
    ///     Chrome’s proxy prefetch UA
    /// </summary>
    [JsonPropertyName(name: "user_agent")]
    public string UserAgent { set; get; } = "prefetch-proxy";

    /// <summary>
    ///     The content of the website has security requirements, and we don't want to risk exposing the site
    /// </summary>
    public bool Disallow { set; get; } = true;
}