namespace DNTCommon.Web.Core;

/// <summary>
///     Notifies when the connection underlying this request is aborted and thus request operations should be canceled.
/// </summary>
public class AsyncLocalCancellationContext : IAsyncLocalCancellationContext
{
    // این متغیر استاتیک است اما مقدارش برای هر درخواست ایزوله است
    private readonly AsyncLocal<CancellationToken> _token = new();

    /// <summary>
    ///     Notifies when the connection underlying this request is aborted and thus request operations should be canceled.
    ///     Use it in methods like .FindAsync(id , _canContext.CurrentToken).
    /// </summary>
    public CancellationToken CurrentToken => _token.Value;

    /// <summary>
    ///     Sets the CancellationToken at the beginning of a request
    /// </summary>
    /// <param name="token"></param>
    public void SetToken(CancellationToken token) => _token.Value = token;
}
