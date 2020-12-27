using System;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Auto Retries Policy
    /// </summary>
    public class AutoRetriesPolicy
    {
        /// <summary>
        /// How many time the downloader service should retries if an error has occurred.
        /// </summary>
        public int MaxRequestAutoRetries { set; get; }

        /// <summary>
        /// How much time the downloader service should wait between retries if an error has occurred.
        /// </summary>
        public TimeSpan AutoRetriesDelay { set; get; }
    }
}