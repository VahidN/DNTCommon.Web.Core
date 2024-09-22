using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
///     `Chrome Privacy Preserving Prefetch Proxy` address (/.well-known/traffic-advice) handler
///     https://developer.chrome.com/blog/private-prefetch-proxy/
/// </summary>
[ApiController]
[AllowAnonymous]
[Route(template: "/.well-known")]
public class PrefetchProxyController : ControllerBase
{
    /// <summary>
    ///     The content of the website has security requirements, and we don't want to risk exposing the site
    /// </summary>
    [HttpGet(template: "traffic-advice")]
    [Produces(contentType: "application/trafficadvice+json")]
    public IActionResult TrafficAdvice()
        => Ok(new[]
        {
            new PrefetchProxyTrafficAdvice()
        });
}