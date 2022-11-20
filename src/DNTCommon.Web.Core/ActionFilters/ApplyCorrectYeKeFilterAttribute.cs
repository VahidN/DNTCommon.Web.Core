using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///     Fixes common writing mistakes caused by using a bad keyboard layout, such as replacing Arabic Ye with Persian one
///     and so on ...
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class ApplyCorrectYeKeFilterAttribute : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext? context)
    {
        context?.CleanupActionStringValues(data => data.ApplyCorrectYeKe());
    }
}