using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     Disposable Extensions
/// </summary>
public static class DisposableExtensions
{
    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public static void TryDisposeSafe(this IDisposable? disposable, ILogger? logger = null, string? message = null)
    {
        if (disposable == null)
        {
            return;
        }

        try
        {
            disposable.Dispose();
            disposable = null;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "{Error}", message ?? "TryDisposeSafe Error");
        }
    }
}