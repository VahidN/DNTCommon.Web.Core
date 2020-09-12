using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// CSP config
    /// </summary>
    public class ContentSecurityPolicyConfig
    {
        /// <summary>
        /// CSP options. Each options should be specified in one line.
        /// </summary>
        public string[] Options { set; get; }
    }

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

        private string getContentSecurityPolicyValue(ContentSecurityPolicyConfig config, string errorLogUri)
        {
            if (config.Options == null || config.Options.Length == 0)
            {
                throw new NullReferenceException("Please set the `ContentSecurityPolicyConfig:Options` value in `appsettings.json` file.");
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
                        controller: nameof(CspReportController).Replace("Controller", string.Empty));
                context.Response.Headers.Add(
                    ContentSecurityPolicy,
                    getContentSecurityPolicyValue(config.Value, errorLogUri));
            }
            return _next(context);
        }
    }

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
}