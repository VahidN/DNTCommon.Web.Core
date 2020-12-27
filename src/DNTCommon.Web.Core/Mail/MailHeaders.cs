namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Custom mail headers
    /// </summary>
    public class MailHeaders
    {
        /// <summary>
        /// Gets or sets the message identifier.
        /// The Message-Id is meant to be a globally unique identifier for a message. MimeKit.Utils.MimeUtils.GenerateMessageId can be used to generate this value.
        /// The message identifier.
        /// </summary>
        public string? MessageId { set; get; }

        /// <summary>
        /// Gets or sets the Message-Id that this message is in reply to.
        /// If the message is a reply to another message, it will typically use the In-Reply-To header to specify the Message-Id of the original message being replied to.
        /// The message id that this message is in reply to.
        /// </summary>
        public string? InReplyTo { set; get; }

        /// <summary>
        /// Gets or sets the list of references to other messages.
        /// The References header contains a chain of Message-Ids back to the original message that started the thread.
        /// The references.
        /// </summary>
        public string? References { set; get; }
    }
}