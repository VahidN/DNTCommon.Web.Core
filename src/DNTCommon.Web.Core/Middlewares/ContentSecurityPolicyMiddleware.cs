using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
/// Content Security Policy Middleware
/// </summary>
public class ContentSecurityPolicyMiddleware
{
    private const string XFrameOptions = "X-Frame-Options";
    private const string XXssProtection = "X-Xss-Protection";
    private const string XContentTypeOptions = "X-Content-Type-Options";
    private const string ContentSecurityPolicy = "Content-Security-Policy";

    private readonly RequestDelegate _next;

    /// <summary>
    /// Content Security Policy Middleware
    /// </summary>
    public ContentSecurityPolicyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    private static string getContentSecurityPolicyValue(ContentSecurityPolicyConfig config, string errorLogUri)
    {
        if (config.Options == null || config.Options.Count == 0)
        {
            throw new InvalidOperationException("Please set the `ContentSecurityPolicyConfig:Options` value in `appsettings.json` file.");
        }
        var options = string.Join("; ", config.Options);
        return $"{options}; report-uri {errorLogUri}";
    }

    /// <summary>
    /// Content Security Policy Middleware pipeline
    /// </summary>
    public Task Invoke(HttpContext context, IOptionsSnapshot<ContentSecurityPolicyConfig> config)
    {
        if (config == null || config.Value == null || config.Value.Options == null)
        {
            throw new ArgumentNullException(nameof(config), "Please add ContentSecurityPolicyConfig to your appsettings.json file.");
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.Response.Headers.ContainsKey(XFrameOptions))
        {
            context.Response.Headers.Add(XFrameOptions, "SAMEORIGIN");
        }

        if (!context.Response.Headers.ContainsKey(XXssProtection))
        {
            context.Response.Headers.Add(XXssProtection, "1; mode=block");
        }

        if (!context.Response.Headers.ContainsKey(XContentTypeOptions))
        {
            context.Response.Headers.Add(XContentTypeOptions, "nosniff"); // Refused to execute script from '<URL>' because its MIME type ('') is not executable, and strict MIME type checking is enabled.
        }

        if (!context.Response.Headers.ContainsKey(ContentSecurityPolicy))
        {
            var errorLogUri = context.GetGenericActionUrl(
                    action: nameof(CspReportController.Log),
                    controller: nameof(CspReportController).Replace("Controller", string.Empty, StringComparison.Ordinal));
            if (errorLogUri is not null)
            {
                context.Response.Headers.Add(
                    ContentSecurityPolicy,
                    getContentSecurityPolicyValue(config.Value, errorLogUri));
            }
        }
        return _next(context);
    }
}