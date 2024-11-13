using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core;

/// <summary>
///     Sets the CultureInfo.CurrentCulture and CultureInfo.CurrentUICulture to the specified culture.
/// </summary>
/// <remarks>
///     Sets the CultureInfo.CurrentCulture and CultureInfo.CurrentUICulture to the specified culture.
/// </remarks>
public sealed class CustomLanguageActionFilterAttribute(string culture) : ActionFilterAttribute
{
    private readonly string _culture = culture ?? throw new ArgumentNullException(nameof(culture));

    /// <summary>
    ///     Initializes a new instance of the CultureInfo class based on the culture specified by name.
    /// </summary>
    public string Culture { get; } = culture;

    /// <summary>
    ///     Sets the CultureInfo.CurrentCulture and CultureInfo.CurrentUICulture to the specified culture.
    /// </summary>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        CultureInfo.CurrentCulture = new CultureInfo(_culture);
        CultureInfo.CurrentUICulture = new CultureInfo(_culture);
        base.OnActionExecuting(context);
    }
}