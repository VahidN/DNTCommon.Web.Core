using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
///     Content Security Policy Middleware
/// </summary>
public class ContentSecurityPolicyMiddleware
{
    private const string XFrameOptions = "X-Frame-Options";
    private const string XXssProtection = "X-Xss-Protection";
    private const string XContentTypeOptions = "X-Content-Type-Options";
    private const string ContentSecurityPolicy = "Content-Security-Policy";

    private readonly RequestDelegate _next;

    /// <summary>
    ///     Content Security Policy Middleware
    /// </summary>
    public ContentSecurityPolicyMiddleware(RequestDelegate next) => _next = next;

    private static string GetContentSecurityPolicyValue(ContentSecurityPolicyConfig config, string errorLogUri)
    {
        if (config.Options == null || config.Options.Count == 0)
        {
            throw new InvalidOperationException(
                "Please set the `ContentSecurityPolicyConfig:Options` value in `appsettings.json` file.");
        }

        var options = string.Join("; ", config.Options);

        return $"{options}; report-uri {errorLogUri}";
    }

    /// <summary>
    ///     Content Security Policy Middleware pipeline
    /// </summary>
    public Task Invoke(HttpContext context, IOptionsSnapshot<ContentSecurityPolicyConfig> config)
    {
        if (config?.Value?.Options == null)
        {
            throw new ArgumentNullException(nameof(config),
                "Please add ContentSecurityPolicyConfig to your appsettings.json file.");
        }

        ArgumentNullException.ThrowIfNull(context);

        context.Response.OnStarting(() =>
        {
            var response = context.Response;

            if (!response.Headers.ContainsKey(XFrameOptions))
            {
                response.Headers.Append(XFrameOptions, "SAMEORIGIN");
            }

            if (!response.Headers.ContainsKey(XXssProtection))
            {
                response.Headers.Append(XXssProtection, "1; mode=block");
            }

            if (!response.Headers.ContainsKey(XContentTypeOptions))
            {
                response.Headers.Append(XContentTypeOptions,
                    "nosniff"); // Refused to execute script from '<URL>' because its MIME type ('') is not executable, and strict MIME type checking is enabled.
            }

            if (!response.Headers.ContainsKey(ContentSecurityPolicy))
            {
                var errorLogUri = context.GetGenericActionUrl(nameof(CspReportController.Log),
                    nameof(CspReportController).Replace("Controller", string.Empty, StringComparison.Ordinal));

                if (errorLogUri is not null)
                {
                    response.Headers.Append(ContentSecurityPolicy,
                        GetContentSecurityPolicyValue(config.Value, errorLogUri));
                }
            }

            return Task.CompletedTask;
        });

        return _next(context);
    }
}