using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     A better IStringLocalizer provider with errors logging.
/// </summary>
/// <remarks>
///     A better IStringLocalizer provider with errors logging.
/// </remarks>
public class SharedResourceService(
    IStringLocalizer sharedHtmlLocalizer,
    IHttpContextAccessor httpContextAccessor,
    ILogger<SharedResourceService> logger,
    IAntiXssService antiXssService) : ISharedResourceService
{
    private readonly IHttpContextAccessor _httpContextAccessor =
        httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    private readonly ILogger<SharedResourceService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly IStringLocalizer _sharedLocalizer =
        sharedHtmlLocalizer ?? throw new ArgumentNullException(nameof(sharedHtmlLocalizer));

    /// <summary>
    ///     Gets all string resources.
    /// </summary>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        => _sharedLocalizer.GetAllStrings(includeParentCultures);

    /// <summary>
    ///     Gets the string resource with the given name.
    /// </summary>
    public string? this[string index] => GetString(index);

    /// <summary>
    ///     Gets the string resource with the given name and formatted with the supplied arguments.
    /// </summary>
    public string? GetString(string name, params object[] arguments)
    {
        var result = _sharedLocalizer.GetString(name, arguments);
        logError(name, result);

        return result;
    }

    /// <summary>
    ///     Gets the string resource with the given name.
    /// </summary>
    public string? GetString(string name)
    {
        var result = _sharedLocalizer.GetString(name);
        logError(name, result);

        return result;
    }

    private void logError(string name, LocalizedString result)
    {
        if (result.ResourceNotFound)
        {
            var acceptLanguage = _httpContextAccessor?.HttpContext?.Request?.Headers[key: "Accept-Language"];

            _logger.LogError(
                message:
                "The localization resource with Accept-Language:`{AcceptLanguage}` & ID:`{Name}` not found. SearchedLocation: `{ResultSearchedLocation}`.",
                antiXssService.GetSanitizedHtml(acceptLanguage), name, result.SearchedLocation);
        }
    }
}