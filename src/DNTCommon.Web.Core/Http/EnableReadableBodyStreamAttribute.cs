using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///     To read request body in an asp.net core webapi controller
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class EnableReadableBodyStreamAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    ///     To read request body in an asp.net core webapi controller
    /// </summary>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.HttpContext.Request.EnableBuffering();
    }
}
