using Microsoft.AspNetCore.Builder;

namespace DNTCommon.Web.Core;

/// <summary>
/// CSP Extensions
/// </summary>
public static class ContentSecurityPolicyMiddlewareExtensions
{
    /// <summary>
    /// Make sure you add this code BEFORE app.UseStaticFiles();,
    /// otherwise the headers will not be applied to your static files.
    /// </summary>
    public static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ContentSecurityPolicyMiddleware>();
    }
}