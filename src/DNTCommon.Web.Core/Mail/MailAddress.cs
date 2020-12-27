namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Defines an email address
    /// </summary>
    public class MailAddress
    {
        /// <summary>
        /// The recipient's name
        /// </summary>
        public string ToName { set; get; } = default!;

        /// <summary>
        /// The recipient's email address
        /// </summary>
        public string ToAddress { set; get; } = default!;
    }
}