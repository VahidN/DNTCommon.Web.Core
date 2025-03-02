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
    public static void TryDisposeSafe<T>(this T? obj, ILogger? logger = null, string? message = null)
        where T : class
    {
        try
        {
            if (obj is IDisposable disposable)
            {
#pragma warning disable IDISP007
                disposable.Dispose();
#pragma warning restore IDISP007
                obj = null;
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Demystify(), message: "{Error}", message ?? "TryDisposeSafe Error");
        }
    }
}
