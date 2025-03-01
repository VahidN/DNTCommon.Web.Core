using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Exception Handler Extension
/// </summary>
public static class ExceptionHandlerExtension
{
    /// <summary>
    ///     A customized version of app.UseExceptionHandler
    /// </summary>
    public static void UseApiExceptionHandler(this IApplicationBuilder app, bool isDevelopment)
        => app.UseExceptionHandler(appBuilder =>
        {
            appBuilder.Run(async context => { await ExceptionHandlerAsync(isDevelopment, context); });
        });

    private static async Task ExceptionHandlerAsync(bool isDevelopment, HttpContext context)
    {
        var error = context.Features.Get<IExceptionHandlerFeature>();
        var exception = error?.Error;

        if (exception is null)
        {
            return;
        }

        switch (exception)
        {
            case UnauthorizedAccessException:
                await ShowErrorAsync(HttpStatusCode.Unauthorized, message: "You don't have access to this resource!");

                break;
            default:
                await ShowErrorAsync(HttpStatusCode.InternalServerError, message: "Unexpected error! Try again later.");

                break;
        }

        Task ShowErrorAsync(HttpStatusCode statusCode, string message)
        {
            addCorsHeaders();

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            return isDevelopment
                ? ShowDevelopmentErrorAsync(statusCode, message)
                : ShowProductionErrorAsync(statusCode, message);
        }

        Task ShowDevelopmentErrorAsync(HttpStatusCode statusCode, string message)
        {
            var exceptionMessage = exception.Demystify().ToString() ?? "Unexpected error! Try again later.";

            return context.Response.WriteAsync(JsonSerializer.Serialize(new ApiErrorDto
            {
                StatusCode = (int)statusCode,
                Message = $"{message}{Environment.NewLine}{exceptionMessage}"
            }), Encoding.UTF8);
        }

        Task ShowProductionErrorAsync(HttpStatusCode statusCode, string message)
            =>

                // NOTE!
                // Don't show the real `exception.Message` in production for security reasons!
                // Attackers shouldn't be able to debug our site remotely!
                context.Response.WriteAsync(JsonSerializer.Serialize(new ApiErrorDto
                {
                    StatusCode = (int)statusCode,
                    Message = message
                }), Encoding.UTF8);

        void addCorsHeaders()
        {
            if (!context.Response.Headers.ContainsKey(key: "Access-Control-Allow-Origin"))
            {
                context.Response.Headers.Append(key: "Access-Control-Allow-Origin", value: "*");
            }

            if (!context.Response.Headers.ContainsKey(key: "Access-Control-Allow-Headers"))
            {
                context.Response.Headers.Append(key: "Access-Control-Allow-Headers",
                    value: "Origin, X-Requested-With, Content-Type, Accept");
            }
        }
    }
}
