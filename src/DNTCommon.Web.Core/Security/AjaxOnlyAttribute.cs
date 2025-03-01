using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace DNTCommon.Web.Core;

/// <summary>
///     Determines whether the HttpRequest's X-Requested-With header has XMLHttpRequest value.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AjaxOnlyAttribute : ActionMethodSelectorAttribute
{
    /// <summary>
    ///     Determines whether the action selection is valid for the specified route context.
    /// </summary>
    public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
    {
        ArgumentNullException.ThrowIfNull(routeContext);

        return routeContext.HttpContext.Request.IsAjaxRequest();
    }
}
