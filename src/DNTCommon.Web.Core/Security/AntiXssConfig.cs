using System;
using System.Collections.Generic;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// AntiXss Config
    /// </summary>
    public class AntiXssConfig
    {
        /// <summary>
        /// List of allowed HTML tags and their attributes
        /// </summary>
        public IReadOnlyCollection<ValidHtmlTag> ValidHtmlTags { set; get; } = new List<ValidHtmlTag>();

        /// <summary>
        /// If an attribute's value contains one of these characters, it will be removed.
        /// </summary>
        public ISet<string> UnsafeAttributeValueCharacters { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}