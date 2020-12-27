using System.Collections.Generic;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// CSP config
    /// </summary>
    public class ContentSecurityPolicyConfig
    {
        /// <summary>
        /// CSP options. Each options should be specified in one line.
        /// </summary>
        public IList<string> Options { get; } = new List<string>();
    }
}