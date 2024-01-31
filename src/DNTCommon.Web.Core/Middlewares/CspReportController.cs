using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Logs the ContentSecurityPolicy errors
/// </summary>
[ApiController, AllowAnonymous, Route("api/[controller]")]
public class CspReportController : ControllerBase
{
    private readonly ILogger<CspReportController> _logger;

    /// <summary>
    ///     Logs the ContentSecurityPolicy errors
    /// </summary>
    public CspReportController(ILogger<CspReportController> logger)
        => _logger = logger;

    /// <summary>
    ///     Logs the ContentSecurityPolicy errors
    /// </summary>
    [HttpPost("[action]"), EnableReadableBodyStream]
    public async Task<IActionResult> Log()
    {
        using (var bodyReader = new StreamReader(HttpContext.Request.Body))
        {
            var body = await bodyReader.ReadToEndAsync();
            _logger.LogError("Content Security Policy Error: {Body}", body);
        }

        return Ok();
    }
}