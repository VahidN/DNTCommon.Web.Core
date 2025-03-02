using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Logs exception information for a ErrorBoundary component.
/// </summary>
public class ErrorBoundaryLoggingService(
    ILogger<ErrorBoundaryLoggingService> logger,
    IHttpContextAccessor httpContextAccessor,
    IEnhancedStackTraceService enhancedStackTraceService) : IErrorBoundaryLogger
{
    /// <inheritdoc />
    public ValueTask LogErrorAsync(Exception exception)
    {
        var stackTrace = httpContextAccessor.HttpContext is null
            ? enhancedStackTraceService.GetCurrentStackTrace(_ => false)
            : "";

        logger.LogError(exception.Demystify(), message: "Unhandled exception rendering component {StackTrace}",
            stackTrace);

        return ValueTask.CompletedTask;
    }
}
