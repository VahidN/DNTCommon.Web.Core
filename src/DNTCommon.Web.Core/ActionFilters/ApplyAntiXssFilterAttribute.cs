using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     An Anti Xss ActionFilter
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class ApplyAntiXssFilterAttribute : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext? context)
    {
        if (context is null)
        {
            return;
        }

        var antiXssService = context.HttpContext.RequestServices.GetRequiredService<IAntiXssService>();
        context.CleanupActionStringValues(data => antiXssService.GetSanitizedHtml(data));
    }
}