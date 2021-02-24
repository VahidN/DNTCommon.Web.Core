using System.Threading.Tasks;
using System.Collections.Generic;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// A web mail service using the `MailKit` library.
    /// </summary>
    public interface IWebMailService
    {
        /// <summary>
        /// Sends an email using the `MailKit` library.
        /// </summary>
        Task SendEmailAsync(
            SmtpConfig smtpConfig,
            IEnumerable<MailAddress> emails,
            string subject,
            string message,
            IEnumerable<MailAddress>? blindCarpbonCopies = null,
            IEnumerable<MailAddress>? carpbonCopies = null,
            IEnumerable<MailAddress>? replyTos = null,
            DelayDelivery? delayDelivery = null,
            IEnumerable<string>? attachmentFiles = null,
            MailHeaders? headers = null,
            bool shouldValidateServerCertificate = true);

        /// <summary>
        /// Sends an email using the `MailKit` library.
        /// This method converts a razor template file to an string and then uses it as the email's message.
        /// </summary>
        Task SendEmailAsync<T>(
            SmtpConfig smtpConfig,
            IEnumerable<MailAddress> emails,
            string subject,
            string viewNameOrPath,
            T viewModel,
            IEnumerable<MailAddress>? blindCarpbonCopies = null,
            IEnumerable<MailAddress>? carpbonCopies = null,
            IEnumerable<MailAddress>? replyTos = null,
            DelayDelivery? delayDelivery = null,
            IEnumerable<string>? attachmentFiles = null,
            MailHeaders? headers = null,
            bool shouldValidateServerCertificate = true);
    }
}