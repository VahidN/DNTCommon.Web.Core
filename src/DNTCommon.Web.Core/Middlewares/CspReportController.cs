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
    [EnableReadableBodyStream]
    public async Task<IActionResult> Log()
    {
        using (var bodyReader = new StreamReader(HttpContext.Request.Body))
        {
            var body = await bodyReader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return Ok();
            }

            cacheService.GetOrAdd(body.Md5Hash(), () =>
            {
                logger.LogError(message: "Content Security Policy Error: {Body}, {Request}",
                    antiXssService.GetSanitizedHtml(body), HttpContext.Request.LogRequest(responseCode: 200));

                return body;
            }, DateTimeOffset.UtcNow.AddMinutes(minutes: 7));
        }

        return Ok();
    }
}