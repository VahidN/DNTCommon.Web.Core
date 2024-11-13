using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///     Allows having a custom AntiforgeryValidationException Error Page
/// </summary>
/// <param name="redirectUrl"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class ValidateAntiForgeryTokenFailedAttribute(
#if NET_9 || NET_8 || NET_7
    [StringSyntax(syntax: "Uri")]
#endif
    string redirectUrl) : Attribute, IAlwaysRunResultFilter
{
    /// <summary>
    ///     Redirect URL after an IAntiforgeryValidationFailedResult
    /// </summary>
    public string RedirectUrl { get; } = default!;

    /// <inheritdoc />
    public void OnResultExecuted(ResultExecutedContext context)
    {
    }

    /// <inheritdoc />
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context?.Result is IAntiforgeryValidationFailedResult)
        {
            context.Result = new RedirectResult(redirectUrl);
        }
    }
}