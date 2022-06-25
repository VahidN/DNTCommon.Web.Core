using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
/// To read request body in an asp.net core webapi controller
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class EnableReadableBodyStreamAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>
    /// To read request body in an asp.net core webapi controller
    /// </summary>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.HttpContext.Request.EnableBuffering();
    }
}