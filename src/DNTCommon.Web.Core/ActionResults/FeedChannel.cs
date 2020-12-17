using System.Collections.Generic;
using System.Linq;

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
        public string FeedTitle { set; get; } = default!;

        /// <summary>
        /// Feed's Description
        /// </summary>
        public string FeedDescription { set; get; } = default!;

        /// <summary>
        /// Feed's Copyright
        /// </summary>
        public string FeedCopyright { set; get; } = default!;

        /// <summary>
        /// An optional feed's image path
        /// </summary>
        public string FeedImageContentPath { set; get; } = default!;

        /// <summary>
        /// An optional feed's image title
        /// </summary>
        public string FeedImageTitle { set; get; } = default!;

        /// <summary>
        /// Feed's RSS Items
        /// </summary>
        public IEnumerable<FeedItem> RssItems { set; get; } = Enumerable.Empty<FeedItem>();

        /// <summary>
        /// Feed language's culture name such as en-US or fa-IR
        /// </summary>
        public string CultureName { set; get; } = default!;
    }
}