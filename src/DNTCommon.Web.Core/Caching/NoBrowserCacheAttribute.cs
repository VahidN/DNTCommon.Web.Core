using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///     Sets `no-cache`, `must-revalidate`, `no-store` headers for the current `Response`.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class NoBrowserCacheAttribute : ActionFilterAttribute
{
    /// <summary>
    ///     surrounds execution of the action
    /// </summary>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.HttpContext.DisableBrowserCache();
        base.OnResultExecuting(context);
    }
}
