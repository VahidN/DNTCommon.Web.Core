namespace DNTCommon.Web.Core;

/// <summary>
///     Notifies when the connection underlying this request is aborted and thus request operations should be canceled.
/// </summary>
public interface IAsyncLocalCancellationContext
{
    /// <summary>
    ///     Notifies when the connection underlying this request is aborted and thus request operations should be canceled.
    ///     Use it in methods like .FindAsync(id , _canContext.CurrentToken).
    /// </summary>
    CancellationToken CurrentToken { get; }

    /// <summary>
    ///     Sets the CancellationToken at the beginning of a request
    /// </summary>
    void SetToken(CancellationToken token);
}
