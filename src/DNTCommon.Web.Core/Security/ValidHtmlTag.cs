using System;
using System.Collections.Generic;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Represents a valid HTML tag for the AntiXss
    /// </summary>
    public class ValidHtmlTag
    {
        /// <summary>
        /// A valid tag name
        /// </summary>
        public string Tag { set; get; } = default!;

        /// <summary>
        /// Valid tag's attributes
        /// </summary>
        public ISet<string> Attributes { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}