using Microsoft.AspNetCore.Components;

namespace DNTCommon.Web.Core;

/// <summary>
///     A web mail service using the `MailKit` library.
/// </summary>
public interface IWebMailService
{
    /// <summary>
    ///     Queues sending an email using the `MailKit` library and IBackgroundQueueService.
    /// </summary>
    void BackgroundQueueSendEmail(SmtpConfig smtpConfig,
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
    ///     Sends an email using the `MailKit` library immediately.
    /// </summary>
    Task SendEmailAsync(SmtpConfig smtpConfig,
        IEnumerable<MailAddress> emails,
        string subject,
        string message,
        IEnumerable<MailAddress>? blindCarpbonCopies = null,
        IEnumerable<MailAddress>? carpbonCopies = null,
        IEnumerable<MailAddress>? replyTos = null,
        DelayDelivery? delayDelivery = null,
        IEnumerable<string>? attachmentFiles = null,
        MailHeaders? headers = null,
        bool shouldValidateServerCertificate = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Queues sending an email using the `MailKit` library and IBackgroundQueueService.
    ///     This method converts a razor template file to a string and then uses it as the email's message.
    /// </summary>
    void BackgroundQueueSendEmail<T>(SmtpConfig smtpConfig,
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

    /// <summary>
    ///     Sends an email using the `MailKit` library immediately.
    ///     This method converts a razor template file to a string and then uses it as the email's message.
    /// </summary>
    Task SendEmailAsync<T>(SmtpConfig smtpConfig,
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
        bool shouldValidateServerCertificate = true,
        CancellationToken cancellationToken = default);

#if NET_10 || NET_9 || NET_8
    /// <summary>
    ///     Sends an email using the `MailKit` library immediately.
    ///     This method converts a blazor .razor template file to a string and then uses it as the email's message.
    /// </summary>
    Task SendEmailAsync<T>(SmtpConfig smtpConfig,
        IEnumerable<MailAddress> emails,
        string subject,
        IDictionary<string, object?> viewModel,
        IEnumerable<MailAddress>? blindCarpbonCopies = null,
        IEnumerable<MailAddress>? carpbonCopies = null,
        IEnumerable<MailAddress>? replyTos = null,
        DelayDelivery? delayDelivery = null,
        IEnumerable<string>? attachmentFiles = null,
        MailHeaders? headers = null,
        bool shouldValidateServerCertificate = true,
        CancellationToken cancellationToken = default)
        where T : IComponent;

    /// <summary>
    ///     Queues sending an email using the `MailKit` library and IBackgroundQueueService.
    ///     This method converts a blazor .razor template file to a string and then uses it as the email's message.
    /// </summary>
    void BackgroundQueueSendEmail<T>(SmtpConfig smtpConfig,
        IEnumerable<MailAddress> emails,
        string subject,
        IDictionary<string, object?> viewModel,
        IEnumerable<MailAddress>? blindCarpbonCopies = null,
        IEnumerable<MailAddress>? carpbonCopies = null,
        IEnumerable<MailAddress>? replyTos = null,
        DelayDelivery? delayDelivery = null,
        IEnumerable<string>? attachmentFiles = null,
        MailHeaders? headers = null,
        bool shouldValidateServerCertificate = true)
        where T : IComponent;

#endif
}
