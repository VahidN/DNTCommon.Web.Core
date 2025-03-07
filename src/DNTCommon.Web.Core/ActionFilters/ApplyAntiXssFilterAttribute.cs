using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
/// An ASP.NET Core Action Filter Attribute that applies Anti-XSS sanitization to the input parameters of the decorated action method or controller.
/// It uses an <see cref="IAntiXssService"/> to sanitize the input data.
/// </summary>
/// <remarks>
/// This attribute can be applied to either a specific action method or an entire controller.
/// When applied, it intercepts the action execution pipeline and sanitizes the string-based input parameters before they are processed by the action method.
/// This helps prevent Cross-Site Scripting (XSS) vulnerabilities by encoding or removing potentially malicious HTML or JavaScript code from the input.
/// </remarks>
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