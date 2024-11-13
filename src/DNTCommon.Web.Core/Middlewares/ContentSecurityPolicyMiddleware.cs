using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
///     Content Security Policy Middleware
/// </summary>
/// <remarks>
///     Content Security Policy Middleware
/// </remarks>
public class ContentSecurityPolicyMiddleware(RequestDelegate next)
{
    private const string XFrameOptions = "X-Frame-Options";
    private const string XXssProtection = "X-Xss-Protection";
    private const string XContentTypeOptions = "X-Content-Type-Options";
    private const string ContentSecurityPolicy = "Content-Security-Policy";

    private static string GetContentSecurityPolicyValue(ContentSecurityPolicyConfig config, string errorLogUri)
    {
        if (config.Options == null || config.Options.Count == 0)
        {
            throw new InvalidOperationException(
                message: "Please set the `ContentSecurityPolicyConfig:Options` value in `appsettings.json` file.");
        }

        var options = string.Join(separator: "; ", config.Options);

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
                message: "Please add ContentSecurityPolicyConfig to your appsettings.json file.");
        }

        ArgumentNullException.ThrowIfNull(context);

        context.Response.OnStarting(() =>
        {
            var response = context.Response;

            if (!response.Headers.ContainsKey(XFrameOptions))
            {
                response.Headers.Append(XFrameOptions, value: "SAMEORIGIN");
            }

            if (!response.Headers.ContainsKey(XXssProtection))
            {
                response.Headers.Append(XXssProtection, value: "1; mode=block");
            }

            if (!response.Headers.ContainsKey(XContentTypeOptions))
            {
                response.Headers.Append(XContentTypeOptions,
                    value: "nosniff"); // Refused to execute script from '<URL>' because its MIME type ('') is not executable, and strict MIME type checking is enabled.
            }

            if (!response.Headers.ContainsKey(ContentSecurityPolicy))
            {
                var errorLogUri = context.GetGenericActionUrl(nameof(CspReportController.Log),
                    nameof(CspReportController)
                        .Replace(oldValue: "Controller", string.Empty, StringComparison.Ordinal));

                if (errorLogUri is not null)
                {
                    response.Headers.Append(ContentSecurityPolicy,
                        GetContentSecurityPolicyValue(config.Value, errorLogUri));
                }
            }

            return Task.CompletedTask;
        });

        return next(context);
    }
}