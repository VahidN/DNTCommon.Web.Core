using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace DNTCommon.Web.Core;

/// <summary>
///     How to use it: requestLocalizationOptions.RequestCultureProviders.Insert(0, new FaRequestCultureProvider());
/// </summary>
/// <remarks>
///     A provider for determining the culture information of an HttpRequest.
/// </remarks>
public class CustomRequestCultureProvider(string culture) : RequestCultureProvider
{
    /// <summary>
    ///     Determining the culture information of an HttpRequest.
    /// </summary>
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        => Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(culture));
}