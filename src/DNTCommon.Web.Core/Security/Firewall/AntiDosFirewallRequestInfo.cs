using System;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
/// Request Info
/// </summary>
public class AntiDosFirewallRequestInfo
{
    /// <summary>
    /// Client's IP
    /// </summary>
    public string? IP { set; get; }

    /// <summary>
    /// Client's UserAgent
    /// </summary>
    public string UserAgent { set; get; } = string.Empty;

    /// <summary>
    /// Request's URL
    /// </summary>
    public string RawUrl { set; get; } = string.Empty;

    /// <summary>
    /// Request's Referrer
    /// </summary>
    public Uri? UrlReferrer { set; get; }

    /// <summary>
    /// Is local request?
    /// </summary>
    public bool IsLocal { set; get; }

    /// <summary>
    /// Represents HttpRequest and HttpResponse headers
    /// </summary>
    public IHeaderDictionary RequestHeaders { get; } = new HeaderDictionary();

    /// <summary>
    /// Request Info
    /// </summary>
    public AntiDosFirewallRequestInfo(IHeaderDictionary requestHeaders)
    {
        RequestHeaders = requestHeaders;
    }

    /// <summary>
    /// Request Info
    /// </summary>
    public AntiDosFirewallRequestInfo()
    {
    }
}