using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Exception Handler Extension
    /// </summary>
    public static class ExceptionHandlerExtension
    {
        /// <summary>
        /// A customized version of app.UseExceptionHandler
        /// </summary>
        public static void UseApiExceptionHandler(this IApplicationBuilder app, bool isDevelopment)
        {
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = error?.Error;
                    if (exception is null)
                    {
                        return;
                    }

                    switch (exception)
                    {
                        case SecurityTokenExpiredException tokenException:
                            await showError(
                                HttpStatusCode.Unauthorized,
                                $"Your token has been expired at {tokenException.Expires}. Please login again!");
                            break;
                        case UnauthorizedAccessException _:
                            await showError(
                                HttpStatusCode.Unauthorized,
                                "You don't have access to this resource!");
                            break;
                        default:
                            await showError(
                                HttpStatusCode.InternalServerError,
                                "Unexpected error! Try again later.");
                            break;
                    }

                    Task showError(HttpStatusCode statusCode, string message)
                    {
                        addCorsHeaders();

                        context.Response.StatusCode = (int)statusCode;
                        context.Response.ContentType = "application/json";
                        return isDevelopment ?
                            showDevelopmentError(statusCode, message) :
                            showProductionError(statusCode, message);
                    }

                    Task showDevelopmentError(HttpStatusCode statusCode, string message)
                    {
                        var exceptionMessage = exception.ToString() ?? "Unexpected error! Try again later.";
                        return context.Response.WriteAsync(JsonSerializer.Serialize(new ApiErrorDto
                        {
                            StatusCode = (int)statusCode,
                            Message = $"{message}{Environment.NewLine}{exceptionMessage}"
                        }), Encoding.UTF8);
                    }

                    Task showProductionError(HttpStatusCode statusCode, string message)
                    {
                        // NOTE!
                        // Don't show the real `exception.Message` in production for security reasons!
                        // Attackers shouldn't be able to debug our site remotely!
                        return context.Response.WriteAsync(JsonSerializer.Serialize(new ApiErrorDto
                        {
                            StatusCode = (int)statusCode,
                            Message = message
                        }), Encoding.UTF8);
                    }

                    void addCorsHeaders()
                    {
                        if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                        {
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        }

                        if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Headers"))
                        {
                            context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                        }
                    }
                });
            });
        }
    }
}