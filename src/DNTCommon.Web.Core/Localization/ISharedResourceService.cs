using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace DNTCommon.Web.Core
{
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
}