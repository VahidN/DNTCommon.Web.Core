using DNTPersianUtils.Core;
using MailKit.Net.Smtp;
using MimeKit;
#if NET9_0 || NET8_0
using Microsoft.AspNetCore.Components;
#endif

namespace DNTCommon.Web.Core;

/// <summary>
///     A web mail service using the `MailKit` library.
/// </summary>
/// <remarks>
///     A web mail service using the `MailKit` library.
/// </remarks>
public class WebMailService(
    IBackgroundQueueService backgroundQueueService,
    IViewRendererService viewRendererService
#if NET_9 || NET_8
    ,
    IBlazorStaticRendererService blazorStaticRendererService
#endif
) : IWebMailService
{
#if NET_9 || NET_8
    private readonly IBlazorStaticRendererService _blazorStaticRendererService = blazorStaticRendererService ??
        throw new ArgumentNullException(nameof(blazorStaticRendererService));
#endif
    private readonly IViewRendererService _viewRendererService =
        viewRendererService ?? throw new ArgumentNullException(nameof(viewRendererService));

    /// <summary>
    ///     Queues sending an email using the `MailKit` library.
    /// </summary>
    public void BackgroundQueueSendEmail(SmtpConfig smtpConfig,
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
        => backgroundQueueService.QueueBackgroundWorkItem((_, _) => SendEmailAsync(smtpConfig, emails, subject, message,
            blindCarpbonCopies, carpbonCopies, replyTos, delayDelivery, attachmentFiles, headers,
            shouldValidateServerCertificate));

    /// <summary>
    ///     Sends an email using the `MailKit` library.
    ///     This method converts a .chtml razor template file to an string and then uses it as the email's message.
    /// </summary>
    public async Task SendEmailAsync<T>(SmtpConfig smtpConfig,
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

        await SendEmailAsync(smtpConfig, emails, subject, message, blindCarpbonCopies, carpbonCopies, replyTos,
            delayDelivery, attachmentFiles, headers, shouldValidateServerCertificate);
    }

    /// <summary>
    ///     Queues sending an email using the `MailKit` library.
    ///     This method converts a razor template file to a string and then uses it as the email's message.
    /// </summary>
    public void BackgroundQueueSendEmail<T>(SmtpConfig smtpConfig,
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
        => backgroundQueueService.QueueBackgroundWorkItem((_, _) => SendEmailAsync(smtpConfig, emails, subject,
            viewNameOrPath, viewModel, blindCarpbonCopies, carpbonCopies, replyTos, delayDelivery, attachmentFiles,
            headers, shouldValidateServerCertificate));

    /// <summary>
    ///     Sends an email using the `MailKit` library.
    /// </summary>
    public async Task SendEmailAsync(SmtpConfig smtpConfig,
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
        ArgumentNullException.ThrowIfNull(smtpConfig);

        ArgumentNullException.ThrowIfNull(emails);

        if (smtpConfig.UsePickupFolder && !string.IsNullOrWhiteSpace(smtpConfig.PickupFolder))
        {
            await UsePickupFolderAsync(smtpConfig, emails.ToArray(), subject, message, blindCarpbonCopies?.ToArray(),
                carpbonCopies?.ToArray(), replyTos?.ToArray(), attachmentFiles?.ToArray(), headers);
        }
        else
        {
            await SendThisEmailAsync(smtpConfig, emails.ToArray(), subject, message, blindCarpbonCopies?.ToArray(),
                carpbonCopies?.ToArray(), replyTos?.ToArray(), delayDelivery, attachmentFiles?.ToArray(), headers,
                shouldValidateServerCertificate);
        }
    }

    private static async Task SendThisEmailAsync(SmtpConfig smtpConfig,
        ICollection<MailAddress> emails,
        string subject,
        string message,
        ICollection<MailAddress>? blindCarpbonCopies,
        ICollection<MailAddress>? carpbonCopies,
        ICollection<MailAddress>? replyTos,
        DelayDelivery? delayDelivery,
        ICollection<string>? attachmentFiles,
        MailHeaders? headers,
        bool shouldValidateServerCertificate)
    {
        using var client = new SmtpClient();

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

        if (!string.IsNullOrWhiteSpace(smtpConfig.Username) && !string.IsNullOrWhiteSpace(smtpConfig.Password))
        {
            await client.AuthenticateAsync(smtpConfig.Username, smtpConfig.Password);
        }

        var count = 0;

        foreach (var email in emails)
        {
            using (var emailMessage = GetEmailMessage(email.ToName, email.ToAddress, subject, message, attachmentFiles,
                       smtpConfig, headers, blindCarpbonCopies, carpbonCopies, replyTos))
            {
                await client.SendAsync(emailMessage);
            }

            count++;

            if (delayDelivery is not null && count % delayDelivery.NumberOfMessages == 0)
            {
                await Task.Delay(delayDelivery.Delay);
            }
        }

        await client.DisconnectAsync(quit: true);
    }

    private static async Task UsePickupFolderAsync(SmtpConfig smtpConfig,
        ICollection<MailAddress> emails,
        string subject,
        string message,
        ICollection<MailAddress>? blindCarpbonCopies,
        ICollection<MailAddress>? carpbonCopies,
        ICollection<MailAddress>? replyTos,
        ICollection<string>? attachmentFiles,
        MailHeaders? headers)
    {
        const int maxBufferSize = 0x10000; // 64K.

        if (string.IsNullOrWhiteSpace(smtpConfig.PickupFolder))
        {
            return;
        }

        smtpConfig.PickupFolder.CreateSafeDir();

        foreach (var email in emails)
        {
            await SaveFileAsync(smtpConfig, subject, message, blindCarpbonCopies, carpbonCopies, replyTos,
                attachmentFiles, headers, maxBufferSize, email);
        }
    }

    private static async Task SaveFileAsync(SmtpConfig smtpConfig,
        string subject,
        string message,
        ICollection<MailAddress>? blindCarpbonCopies,
        ICollection<MailAddress>? carpbonCopies,
        ICollection<MailAddress>? replyTos,
        ICollection<string>? attachmentFiles,
        MailHeaders? headers,
        int maxBufferSize,
        MailAddress email)
    {
        if (smtpConfig?.PickupFolder is null)
        {
            return;
        }

        var path = Path.Combine(smtpConfig.PickupFolder, $"email-{Guid.NewGuid():N}.eml");

        await using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None,
            maxBufferSize, useAsync: true);

        using var emailMessage = GetEmailMessage(email.ToName, email.ToAddress, subject, message, attachmentFiles,
            smtpConfig, headers, blindCarpbonCopies, carpbonCopies, replyTos);

        await emailMessage.WriteToAsync(stream);
    }

    private static MimeMessage GetEmailMessage(string toName,
        string toAddress,
        string subject,
        string message,
        ICollection<string>? attachmentFiles,
        SmtpConfig smtpConfig,
        MailHeaders? headers,
        ICollection<MailAddress>? blindCarpbonCopies,
        ICollection<MailAddress>? carpbonCopies,
        ICollection<MailAddress>? replyTos)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(smtpConfig.FromName, smtpConfig.FromAddress));
        emailMessage.Subject = subject.ApplyRle();
        emailMessage.To.Add(new MailboxAddress(toName ?? string.Empty, toAddress));

        if (blindCarpbonCopies?.Count > 0)
        {
            foreach (var bcc in blindCarpbonCopies)
            {
                emailMessage.Bcc.Add(new MailboxAddress(bcc.ToName ?? string.Empty, bcc.ToAddress));
            }
        }

        if (carpbonCopies?.Count > 0)
        {
            foreach (var cc in carpbonCopies)
            {
                emailMessage.Cc.Add(new MailboxAddress(cc.ToName ?? string.Empty, cc.ToAddress));
            }
        }

        if (replyTos?.Count > 0)
        {
            foreach (var rt in replyTos)
            {
                emailMessage.ReplyTo.Add(new MailboxAddress(rt.ToName ?? string.Empty, rt.ToAddress));
            }
        }

        emailMessage.Body = GetMessageBody(message, attachmentFiles);
        AddHeaders(emailMessage, headers, smtpConfig.FromAddress);

        return emailMessage;
    }

    private static void AddHeaders(MimeMessage emailMessage, MailHeaders? headers, string fromAddress)
    {
        if (headers is null)
        {
            return;
        }

        var host = fromAddress.Split(separator: '@')[^1];

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

        if (!string.IsNullOrWhiteSpace(headers.UnSubscribeUrl))
        {
            emailMessage.Headers.Add(field: "List-Unsubscribe",
                string.Format(CultureInfo.InvariantCulture, format: "<{0}>", headers.UnSubscribeUrl));
        }
    }

    private static MimeEntity GetMessageBody(string message, ICollection<string>? attachmentFiles)
    {
        var builder = new BodyBuilder
        {
            HtmlBody = message
        };

        if (attachmentFiles?.Count > 0)
        {
            foreach (var attachmentFile in attachmentFiles)
            {
                if (!string.IsNullOrWhiteSpace(attachmentFile) && File.Exists(attachmentFile))
                {
#pragma warning disable IDISP004
                    builder.Attachments.Add(attachmentFile);
#pragma warning restore IDISP004
                }
            }
        }

        return builder.ToMessageBody();
    }

#if NET_9 || NET_8
    /// <summary>
    ///     Sends an email using the `MailKit` library.
    ///     This method converts a Blazor .razor template file to an string and then uses it as the email's message.
    /// </summary>
    public async Task SendEmailAsync<T>(SmtpConfig smtpConfig,
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
        where T : IComponent
    {
        var message = await _blazorStaticRendererService.StaticRenderComponentAsync<T>(viewModel);

        await SendEmailAsync(smtpConfig, emails, subject, message, blindCarpbonCopies, carpbonCopies, replyTos,
            delayDelivery, attachmentFiles, headers, shouldValidateServerCertificate);
    }

    /// <summary>
    ///     Queues sending an email using the `MailKit` library.
    ///     This method converts a blazor .razor template file to a string and then uses it as the email's message.
    /// </summary>
    public void BackgroundQueueSendEmail<T>(SmtpConfig smtpConfig,
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
        where T : IComponent
        => backgroundQueueService.QueueBackgroundWorkItem((_, _) => SendEmailAsync<T>(smtpConfig, emails, subject,
            viewModel, blindCarpbonCopies, carpbonCopies, replyTos, delayDelivery, attachmentFiles, headers,
            shouldValidateServerCertificate));

#endif
}
