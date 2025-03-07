using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///   Corrects common typographical errors caused by incorrect keyboard layouts,
///   such as replacing the Arabic 'ي' with the Persian 'ی'. It applies proper
///   YeKe (ی and ک) to improve the text's readability and accuracy.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class ApplyCorrectYeKeFilterAttribute : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext? context) => context?.CleanupActionStringValues(data => data.ApplyCorrectYeKe());
}
