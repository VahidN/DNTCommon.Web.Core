using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///   An attribute that allows customizing the response when an Antiforgery token validation fails.
///   It redirects the user to a specified URL upon encountering an <see cref="IAntiforgeryValidationFailedResult"/>.
/// </summary>
/// <remarks>
///   This attribute implements the <see cref="IAlwaysRunResultFilter"/> interface to ensure that it is always executed,
///   regardless of other filters. It checks if the result of the action is an <see cref="IAntiforgeryValidationFailedResult"/>
///   and, if so, replaces the result with a <see cref="RedirectResult"/> that redirects the user to the specified URL.
/// </remarks>
/// <param name="redirectUrl">
///   The URL to redirect to when Antiforgery token validation fails.
///   It's recommended to be a page displaying a user-friendly error message.
/// </param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class ValidateAntiForgeryTokenFailedAttribute(
#if NET_9 || NET_8 || NET_7
    [StringSyntax(syntax: "Uri")]
#endif
    string redirectUrl) : Attribute, IAlwaysRunResultFilter
{
    /// <summary>
    ///   The URL to redirect to when Antiforgery token validation fails.
    ///   It's recommended to be a page displaying a user-friendly error message.
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