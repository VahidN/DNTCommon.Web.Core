using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     https://brockallen.com/2019/01/11/same-site-cookies-asp-net-core-and-external-authentication-providers/
/// </summary>
/// <param name="next"></param>
public class StrictSameSiteCookieFixMiddleware(RequestDelegate next)
{
    // MA0137 : Method returning an awaitable type must use the 'Async' suffix -> well, this is how it should be defined!
    /// <summary>
    ///     Invokes the StrictSameSiteCookieFixMiddleware
    /// </summary>
#pragma warning disable CC001,MA0137
    public async Task Invoke(HttpContext ctx)
#pragma warning restore CC001, MA0137
    {
        ArgumentNullException.ThrowIfNull(ctx);

        var authenticationSchemeProvider = ctx.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
        var authenticationHandlerProvider = ctx.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
        var schemes = await authenticationSchemeProvider.GetRequestHandlerSchemesAsync();

        foreach (var scheme in schemes)
        {
            if (await authenticationHandlerProvider.GetHandlerAsync(ctx, scheme.Name) is not
                    IAuthenticationRequestHandler handler || !await handler.HandleRequestAsync())
            {
                continue;
            }

            string? location = null;

            if (ctx.Response.StatusCode == 302)
            {
                location = ctx.Response.Headers.Location;
            }
            else if (string.Equals(ctx.Request.Method, b: "GET", StringComparison.OrdinalIgnoreCase) &&
                     ctx.Request.Query[key: "skip"].Count == 0)
            {
                location = $"{ctx.Request.Path}{ctx.Request.QueryString}&skip=1";
            }

            if (string.IsNullOrEmpty(location))
            {
                return;
            }

            ctx.Response.ContentType = "text/html";
            ctx.Response.StatusCode = 200;

            var html = $"""
                        <html>
                        <head>
                           <meta http-equiv='refresh' content='0;url={location}' />
                        </head>
                        </html>
                        """;

            await ctx.Response.WriteAsync(html);

            return;
        }

        await next(ctx);
    }
}
