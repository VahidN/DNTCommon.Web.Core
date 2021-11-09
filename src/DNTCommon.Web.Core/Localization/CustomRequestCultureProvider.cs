using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// How to use it: requestLocalizationOptions.RequestCultureProviders.Insert(0, new FaRequestCultureProvider());
    /// </summary>
    public class CustomRequestCultureProvider : RequestCultureProvider
    {
        private readonly string _culture;

        /// <summary>
        /// A provider for determining the culture information of an HttpRequest.
        /// </summary>
        public CustomRequestCultureProvider(string culture)
        {
            _culture = culture;
        }

        /// <summary>
        /// Determining the culture information of an HttpRequest.
        /// </summary>
        public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(_culture));
        }
    }
}