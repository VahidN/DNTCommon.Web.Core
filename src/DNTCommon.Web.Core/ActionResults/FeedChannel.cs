using System.Collections.Generic;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Represents a feed channel
    /// </summary>
    public class FeedChannel
    {
        /// <summary>
        /// Feed's Title
        /// </summary>
        public string FeedTitle { set; get; }

        /// <summary>
        /// Feed's Description
        /// </summary>
        public string FeedDescription { set; get; }

        /// <summary>
        /// Feed's Copyright
        /// </summary>
        public string FeedCopyright { set; get; }

        /// <summary>
        /// An optional feed's image path
        /// </summary>
        public string FeedImageContentPath { set; get; }

        /// <summary>
        /// An optional feed's image title
        /// </summary>
        public string FeedImageTitle { set; get; }

        /// <summary>
        /// Feed's RSS Items
        /// </summary>
        public IList<FeedItem> RssItems { set; get; }

        /// <summary>
        /// Feed language's culture name such as en-US or fa-IR
        /// </summary>
        public string CultureName { set; get; }
    }
}