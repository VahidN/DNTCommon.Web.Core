using System.Globalization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Sets the CultureInfo.CurrentCulture and CultureInfo.CurrentUICulture to the specified culture.
    /// </summary>
    public class CustomLanguageActionFilterAttribute : ActionFilterAttribute
    {
        private readonly string _culture;

        /// <summary>
        /// Sets the CultureInfo.CurrentCulture and CultureInfo.CurrentUICulture to the specified culture.
        /// </summary>
        public CustomLanguageActionFilterAttribute(string culture)
        {
            _culture = culture;
        }

        /// <summary>
        /// Sets the CultureInfo.CurrentCulture and CultureInfo.CurrentUICulture to the specified culture.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            CultureInfo.CurrentCulture = new CultureInfo(_culture);
            CultureInfo.CurrentUICulture = new CultureInfo(_culture);
            base.OnActionExecuting(context);
        }
    }
}