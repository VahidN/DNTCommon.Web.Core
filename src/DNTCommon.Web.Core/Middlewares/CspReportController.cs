using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Logs the ContentSecurityPolicy errors
/// </summary>
/// <remarks>
///     Logs the ContentSecurityPolicy errors
/// </remarks>
[ApiController]
[AllowAnonymous]
[Route(template: "api/[controller]")]
public class CspReportController(
    ILogger<CspReportController> logger,
    IAntiXssService antiXssService,
    ICacheService cacheService) : ControllerBase
{
    /// <summary>
    ///     Logs the ContentSecurityPolicy errors
    /// </summary>
    [HttpPost(template: "[action]")]
    [HttpGet(template: "[action]")]
    [EnableReadableBodyStream]
#pragma warning disable CC005B, CC001
    public async Task<IActionResult> Log(
#pragma warning restore CC001, CC005B
#if !NET_6
        CancellationToken cancellationToken = default
#endif
    )
    {
        if (HttpContext.IsGetRequest())
        {
            return Ok();
        }

        using (var bodyReader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8))
        {
#if !NET_6
            var body = await bodyReader.ReadToEndAsync(cancellationToken);
#else
            var body = await bodyReader.ReadToEndAsync();
#endif

            if (string.IsNullOrWhiteSpace(body))
            {
                return Ok();
            }

            cacheService.GetOrAdd(body.Md5Hash(), nameof(CspReportController), () =>
            {
                if (ShouldBeIgnored(body))
                {
                    return body;
                }

                logger.LogError(message: "Content Security Policy Error: {Body}, {Request}",
                    antiXssService.GetSanitizedHtml(body), HttpContext.Request.LogRequest(responseCode: 200));

                return body;
            }, DateTimeOffset.UtcNow.AddMinutes(minutes: 7));
        }

        return Ok();
    }

    private static bool ShouldBeIgnored(string message)
    {
        string[] errors =
        [
            "\"violated-directive\":\"font-src\"", "moz-extension", "chrome-extension", "ms-browser-extension",
            "google.com"
        ];

        return errors.Any(error => message.Contains(error, StringComparison.OrdinalIgnoreCase));
    }
}
