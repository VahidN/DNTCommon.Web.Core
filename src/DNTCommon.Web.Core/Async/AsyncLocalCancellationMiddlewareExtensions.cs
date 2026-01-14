using Microsoft.AspNetCore.Builder;

namespace DNTCommon.Web.Core;

/// <summary>
///     CancellationMiddleware Extensions
/// </summary>
public static class AsyncLocalCancellationMiddlewareExtensions
{
    /// <summary>
    ///     Adds CancellationMiddleware to the pipeline.
    ///     Notifies when the connection underlying this request is aborted and thus request operations should be canceled.
    ///     Use it in methods like .FindAsync(id , _IAsyncLocalCancellationContext.CurrentToken).
    /// </summary>
    public static IApplicationBuilder UseAsyncLocalCancellation(this IApplicationBuilder builder)
        => builder.UseMiddleware<AsyncLocalCancellationMiddleware>();
}
