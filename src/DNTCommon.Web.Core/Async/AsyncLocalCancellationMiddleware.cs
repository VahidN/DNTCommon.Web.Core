using Microsoft.AspNetCore.Http;

namespace DNTCommon.Web.Core;

/// <summary>
///     Notifies when the connection underlying this request is aborted and thus request operations should be canceled.
/// </summary>
public class AsyncLocalCancellationMiddleware(
    RequestDelegate next,
    IAsyncLocalCancellationContext asyncLocalCancellationContext)
{
#pragma warning disable CC001,MA0137
    public async Task Invoke(HttpContext context)
#pragma warning restore CC001, MA0137
    {
        ArgumentNullException.ThrowIfNull(context);

        // توکن مربوط به این درخواست را درون کانتینر سراسری می‌ریزیم
        asyncLocalCancellationContext.SetToken(context.RequestAborted);

        await next(context);
    }
}
