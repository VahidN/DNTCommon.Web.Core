using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Http Request Info Extensions
    /// </summary>
    public static class SharedResourceServiceExtensions
    {
        /// <summary>
        /// Adds ISharedResourceService to IServiceCollection.
        /// How to use it: services.AddSharedResourceService of SharedResource;
        /// </summary>
        public static IServiceCollection AddSharedResourceService<T>(
                this IServiceCollection services) where T : class
        {
            services.AddScoped<IStringLocalizer>(provider => provider.GetRequiredService<IStringLocalizer<T>>());
            services.TryAddScoped<ISharedResourceService, SharedResourceService>();
            return services;
        }
    }

    /// <summary>
    /// A better IStringLocalizer provider with errors logging.
    /// </summary>
    public interface ISharedResourceService
    {
        /// <summary>
        /// Gets the string resource with the given name.
        /// </summary>
        string? this[string index] { get; }

        /// <summary>
        /// Gets all string resources.
        /// </summary>
        IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures);

        /// <summary>
        /// Gets the string resource with the given name and formatted with the supplied arguments.
        /// </summary>
        string? GetString(string name, params object[] arguments);

        /// <summary>
        /// Gets the string resource with the given name.
        /// </summary>
        string? GetString(string name);
    }

    /// <summary>
    /// A better IStringLocalizer provider with errors logging.
    /// </summary>
    public class SharedResourceService : ISharedResourceService
    {
        private readonly IStringLocalizer _sharedLocalizer;
        private readonly ILogger<SharedResourceService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// A better IStringLocalizer provider with errors logging.
        /// </summary>
        public SharedResourceService(
            IStringLocalizer sharedHtmlLocalizer,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SharedResourceService> logger
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sharedLocalizer = sharedHtmlLocalizer ?? throw new ArgumentNullException(nameof(sharedHtmlLocalizer));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Gets all string resources.
        /// </summary>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _sharedLocalizer.GetAllStrings(includeParentCultures);
        }

        /// <summary>
        /// Gets the string resource with the given name.
        /// </summary>
        public string? this[string index] => GetString(index);

        /// <summary>
        /// Gets the string resource with the given name and formatted with the supplied arguments.
        /// </summary>
        public string? GetString(string name, params object[] arguments)
        {
            var result = _sharedLocalizer.GetString(name, arguments);
            logError(name, result);
            return result;
        }

        private void logError(string name, LocalizedString result)
        {
            if (result.ResourceNotFound)
            {
                var acceptLanguage = _httpContextAccessor?.HttpContext?.Request?.Headers["Accept-Language"];
                _logger.LogError($"The localization resource with Accept-Language:`{acceptLanguage}` & ID:`{name}` not found. SearchedLocation: `{result.SearchedLocation}`.");
            }
        }

        /// <summary>
        /// Gets the string resource with the given name.
        /// </summary>
        public string? GetString(string name)
        {
            var result = _sharedLocalizer.GetString(name);
            logError(name, result);
            return result;
        }
    }
}