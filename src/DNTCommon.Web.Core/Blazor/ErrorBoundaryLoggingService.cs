using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Logs exception information for a ErrorBoundary component.
/// </summary>
public class ErrorBoundaryLoggingService(ILogger<ErrorBoundaryLoggingService> logger) : IErrorBoundaryLogger
{
    /// <inheritdoc />
    public ValueTask LogErrorAsync(Exception exception)
    {
        logger.LogError(exception.Demystify(), message: "Unhandled exception rendering component");

        return ValueTask.CompletedTask;
    }
}