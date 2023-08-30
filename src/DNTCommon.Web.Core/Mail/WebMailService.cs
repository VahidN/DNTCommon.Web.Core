using DNTPersianUtils.Core;
using MailKit.Net.Smtp;
using MimeKit;

namespace DNTCommon.Web.Core;

/// <summary>
///     A web mail service using the `MailKit` library.
/// </summary>
public class WebMailService : IWebMailService
{
    private readonly IViewRendererService _viewRendererService;

    /// <summary>
    ///     A web mail service using the `MailKit` library.
    /// </summary>
    public WebMailService(IViewRendererService viewRendererService) =>
        _viewRendererService = viewRendererService ?? throw new ArgumentNullException(nameof(viewRendererService));

    /// <summary>
    ///     Sends an email using the `MailKit` library.
    ///     This method converts a razor template file to an string and then uses it as the email's message.
    /// </summary>
    public async Task SendEmailAsync<T>(
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
        bool shouldValidateServerCertificate = true)
    {
        var message = await _viewRendererService.RenderViewToStringAsync(viewNameOrPath, viewModel);
        await SendEmailAsync(
                             smtpConfig,
                             emails,
                             subject,
                             message,
                             blindCarpbonCopies,
                             carpbonCopies,
                             replyTos,
                             delayDelivery,
                             attachmentFiles,
                             headers,
                             shouldValidateServerCertificate);
    }

    /// <summary>
    ///     Sends an email using the `MailKit` library.
    /// </summary>
    public async Task SendEmailAsync(
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
        bool shouldValidateServerCertificate = true)
    {
        if (smtpConfig == null)
        {
            throw new ArgumentNullException(nameof(smtpConfig));
        }

        if (emails == null)
        {
            throw new ArgumentNullException(nameof(emails));
        }

        if (smtpConfig.UsePickupFolder && !string.IsNullOrWhiteSpace(smtpConfig.PickupFolder))
        {
            await usePickupFolderAsync(smtpConfig,
                                       emails,
                                       subject,
                                       message,
                                       blindCarpbonCopies,
                                       carpbonCopies,
                                       replyTos,
                                       attachmentFiles,
                                       headers);
        }
        else
        {
            await sendEmailAsync(
                                 smtpConfig,
                                 emails,
                                 subject,
                                 message,
                                 blindCarpbonCopies,
                                 carpbonCopies,
                                 replyTos,
                                 delayDelivery,
                                 attachmentFiles,
                                 headers,
                                 shouldValidateServerCertificate);
        }
    }

    private static async Task sendEmailAsync(
        SmtpConfig smtpConfig,
        IEnumerable<MailAddress> emails,
        string subject,
        string message,
        IEnumerable<MailAddress>? blindCarpbonCopies,
        IEnumerable<MailAddress>? carpbonCopies,
        IEnumerable<MailAddress>? replyTos,
        DelayDelivery? delayDelivery,
        IEnumerable<string>? attachmentFiles,
        MailHeaders? headers,
        bool shouldValidateServerCertificate)
    {
        using (var client = new SmtpClient())
        {
            if (!shouldValidateServerCertificate)
            {
                client.ServerCertificateValidationCallback = (_, _, _, _) => true;
                client.CheckCertificateRevocation = false;
            }

            if (!string.IsNullOrWhiteSpace(smtpConfig.LocalDomain))
            {
                client.LocalDomain = smtpConfig.LocalDomain;
            }

            await client.ConnectAsync(smtpConfig.Server, smtpConfig.Port);
            if (!string.IsNullOrWhiteSpace(smtpConfig.Username) &&
                !string.IsNullOrWhiteSpace(smtpConfig.Password))
            {
                await client.AuthenticateAsync(smtpConfig.Username, smtpConfig.Password);
            }

            var count = 0;
            foreach (var email in emails)
            {
                using (var emailMessage = getEmailMessage(email.ToName,
                                                          email.ToAddress,
                                                          subject,
                                                          message,
                                                          attachmentFiles,
                                                          smtpConfig,
                                                          headers,
                                                          blindCarpbonCopies,
                                                          carpbonCopies,
                                                          replyTos))
                {
                    await client.SendAsync(emailMessage);
                }

                count++;

                if (delayDelivery != null && count % delayDelivery.NumberOfMessages == 0)
                {
                    await Task.Delay(delayDelivery.Delay);
                }
            }

            await client.DisconnectAsync(true);
        }
    }

    private static async Task usePickupFolderAsync(SmtpConfig smtpConfig,
                                                   IEnumerable<MailAddress> emails,
                                                   string subject,
                                                   string message,
                                                   IEnumerable<MailAddress>? blindCarpbonCopies,
                                                   IEnumerable<MailAddress>? carpbonCopies,
                                                   IEnumerable<MailAddress>? replyTos,
                                                   IEnumerable<string>? attachmentFiles,
                                                   MailHeaders? headers)
    {
        const int maxBufferSize = 0x10000; // 64K.

        if (string.IsNullOrWhiteSpace(smtpConfig.PickupFolder))
        {
            return;
        }

        if (!Directory.Exists(smtpConfig.PickupFolder))
        {
            Directory.CreateDirectory(smtpConfig.PickupFolder);
        }

        foreach (var email in emails)
        {
            using (var stream = new FileStream(
                                               Path.Combine(smtpConfig.PickupFolder,
                                                            $"email-{Guid.NewGuid().ToString("N")}.eml"),
                                               FileMode.CreateNew,
                                               FileAccess.Write,
                                               FileShare.None,
                                               maxBufferSize,
                                               true))
            {
                using (var emailMessage = getEmailMessage(email.ToName,
                                                          email.ToAddress,
                                                          subject,
                                                          message,
                                                          attachmentFiles,
                                                          smtpConfig,
                                                          headers,
                                                          blindCarpbonCopies,
                                                          carpbonCopies,
                                                          replyTos))
                {
                    await emailMessage.WriteToAsync(stream);
                }
            }
        }
    }

    private static MimeMessage getEmailMessage(
        string toName,
        string toAddress,
        string subject,
        string message,
        IEnumerable<string>? attachmentFiles,
        SmtpConfig smtpConfig,
        MailHeaders? headers,
        IEnumerable<MailAddress>? blindCarpbonCopies,
        IEnumerable<MailAddress>? carpbonCopies,
        IEnumerable<MailAddress>? replyTos)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(smtpConfig.FromName, smtpConfig.FromAddress));
        emailMessage.Subject = subject.ApplyRle();
        emailMessage.To.Add(new MailboxAddress(toName ?? string.Empty, toAddress));

        if (blindCarpbonCopies?.Any() == true)
        {
            foreach (var bcc in blindCarpbonCopies)
            {
                emailMessage.Bcc.Add(new MailboxAddress(bcc.ToName ?? string.Empty, bcc.ToAddress));
            }
        }

        if (carpbonCopies?.Any() == true)
        {
            foreach (var cc in carpbonCopies)
            {
                emailMessage.Cc.Add(new MailboxAddress(cc.ToName ?? string.Empty, cc.ToAddress));
            }
        }

        if (replyTos?.Any() == true)
        {
            foreach (var rt in replyTos)
            {
                emailMessage.ReplyTo.Add(new MailboxAddress(rt.ToName ?? string.Empty, rt.ToAddress));
            }
        }

        emailMessage.Body = getMessageBody(message, attachmentFiles);
        addHeaders(emailMessage, headers, smtpConfig.FromAddress);

        return emailMessage;
    }

    private static void addHeaders(MimeMessage emailMessage, MailHeaders? headers, string fromAddress)
    {
        if (headers == null)
        {
            return;
        }

        var host = fromAddress.Split('@')[^1];

        if (!string.IsNullOrWhiteSpace(headers.MessageId))
        {
            emailMessage.MessageId = $"<{headers.MessageId}@{host}>";
        }

        if (!string.IsNullOrWhiteSpace(headers.InReplyTo))
        {
            emailMessage.InReplyTo = $"<{headers.InReplyTo}@{host}>";
        }

        if (!string.IsNullOrWhiteSpace(headers.References))
        {
            emailMessage.References.Add($"<{headers.References}@{host}>");
        }
    }

    private static MimeEntity getMessageBody(string message, IEnumerable<string>? attachmentFiles)
    {
        var builder = new BodyBuilder
                      {
                          HtmlBody = message,
                      };
        if (attachmentFiles?.Any() == true)
        {
            foreach (var attachmentFile in attachmentFiles)
            {
                if (!string.IsNullOrWhiteSpace(attachmentFile) && File.Exists(attachmentFile))
                {
                    builder.Attachments.Add(attachmentFile);
                }
            }
        }

        return builder.ToMessageBody();
    }
}