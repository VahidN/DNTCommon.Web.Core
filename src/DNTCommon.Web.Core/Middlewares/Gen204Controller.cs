using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
///     Just to stop `/gen204?invalidResponse` errors!
/// </summary>
[ApiController]
[AllowAnonymous]
[Route(template: "[controller]")]
public class Gen204Controller : ControllerBase
{
    /// <summary>
    ///     Just to stop `/gen204?invalidResponse` errors!
    /// </summary>
    [HttpGet]
    [HttpPost]
    public IActionResult Get() => Content(content: "ok");
}