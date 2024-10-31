using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace DNTCommon.Web.Core;

/// <summary>
///     More info: http://www.dntips.ir/post/2519
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    /// <summary>
    ///     Adds UseSecurityHeaders to IApplicationBuilder.
    ///     Adds middleware to your web application pipeline to automatically add security headers to requests.
    /// </summary>
    public static void UseCsp(this IApplicationBuilder app, IHostEnvironment env, bool enableCrossOriginPolicy)
    {
        var headerPolicyCollection = SecurityHeadersBuilder.GetCsp(env.IsDevelopment(), enableCrossOriginPolicy);
        app.UseSecurityHeaders(headerPolicyCollection);
    }
}