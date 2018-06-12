using System;
using System.Collections.Generic;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// AntiDos Config
    /// </summary>
    public class AntiDosConfig
    {
        /// <summary>
        /// Such as looking for `etc/passwd` in the requested URL.
        /// </summary>
        public ISet<string> UrlAttackVectors { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Such as `google` or `bing`.
        /// </summary>
        public ISet<string> GoodBotsUserAgents { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Such as `asafaweb`.
        /// </summary>
        public ISet<string> BadBotsUserAgents { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Such as `HTTP_ACUNETIX_PRODUCT`.
        /// </summary>
        public ISet<string> BadBotsRequestHeaders { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// List of the permanent banned IPs.
        /// </summary>
        public ISet<string> BannedIPAddressRanges { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// How long a client should be banned in minutes?
        /// </summary>
        public int DurationMin { set; get; }

        /// <summary>
        /// Number of allowed requests per `DurationMin`.
        /// </summary>
        public int AllowedRequests { set; get; }

        /// <summary>
        /// An HTML error message for the banned users.
        /// </summary>
        public string ErrorMessage { set; get; }

        /// <summary>
        /// Should we apply this middleware to the localhost requests?
        /// </summary>
        public bool IgnoreLocalHost { set; get; }
    }
}