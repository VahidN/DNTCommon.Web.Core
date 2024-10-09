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
    public async Task<IActionResult> Log()
    {
        if (HttpContext.IsGetRequest())
        {
            return Ok();
        }

        using (var bodyReader = new StreamReader(HttpContext.Request.Body))
        {
            var body = await bodyReader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return Ok();
            }

            cacheService.GetOrAdd(body.Md5Hash(), () =>
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