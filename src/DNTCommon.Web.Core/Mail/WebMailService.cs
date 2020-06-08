using System;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using DNTPersianUtils.Core;
using System.Linq;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Web Mail Service Extensions
    /// </summary>
    public static class WebMailServiceExtensions
    {
        /// <summary>
        /// Adds IWebMailService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddWebMailService(this IServiceCollection services)
        {
            services.AddScoped<IWebMailService, WebMailService>();
            return services;
        }
    }

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
            IEnumerable<MailAddress> blindCarpbonCopies = null,
            IEnumerable<MailAddress> carpbonCopies = null,
            IEnumerable<MailAddress> replyTos = null,
            DelayDelivery delayDelivery = null,
            IEnumerable<string> attachmentFiles = null,
            MailHeaders headers = null);

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
            IEnumerable<MailAddress> blindCarpbonCopies = null,
            IEnumerable<MailAddress> carpbonCopies = null,
            IEnumerable<MailAddress> replyTos = null,
            DelayDelivery delayDelivery = null,
            IEnumerable<string> attachmentFiles = null,
            MailHeaders headers = null);
    }

    /// <summary>
    /// Delay Delivery
    /// </summary>
    public class DelayDelivery
    {
        /// <summary>
        /// Its default value is 1 second.
        /// </summary>
        public TimeSpan Delay { set; get; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Its default value is after sending 30 messages.
        /// </summary>
        public int NumberOfMessages { set; get; } = 30;
    }

    /// <summary>
    /// Defines an email address
    /// </summary>
    public class MailAddress
    {
        /// <summary>
        /// The recipient's name
        /// </summary>
        public string ToName { set; get; }

        /// <summary>
        /// The recipient's email address
        /// </summary>
        public string ToAddress { set; get; }
    }

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
        public string MessageId { set; get; }

        /// <summary>
        /// Gets or sets the Message-Id that this message is in reply to.
        /// If the message is a reply to another message, it will typically use the In-Reply-To header to specify the Message-Id of the original message being replied to.
        /// The message id that this message is in reply to.
        /// </summary>
        public string InReplyTo { set; get; }

        /// <summary>
        /// Gets or sets the list of references to other messages.
        /// The References header contains a chain of Message-Ids back to the original message that started the thread.
        /// The references.
        /// </summary>
        public string References { set; get; }
    }

    /// <summary>
    /// The SMTP server's config
    /// </summary>
    public class SmtpConfig
    {
        /// <summary>
        /// The host name to connect to.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The optional user name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The optional password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The port to connect to. If the specified port is 0, then the default port will be used.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The local domain is used in the HELO or EHLO commands sent to the SMTP server. If left unset, the local IP address will be used instead.
        /// </summary>
        public string LocalDomain { get; set; }

        /// <summary>
        /// If you set this value to true, the `SendEmailAsync` method won't send this email to the recipient and
        /// saves its content as an .eml file in the `PickupFolder`.
        /// Later you can open this file using Outlook or your web browser to debug your program.
        /// </summary>
        /// <returns></returns>
        public bool UsePickupFolder { get; set; }

        /// <summary>
        /// An optional path to save emails on the local HDD. Its value will be used if you set the `UsePickupFolder` to true.
        /// </summary>
        public string PickupFolder { get; set; }

        /// <summary>
        /// The name of the mailbox.
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// The address of the mailbox.
        /// </summary>
        public string FromAddress { get; set; }
    }

    /// <summary>
    /// A web mail service using the `MailKit` library.
    /// </summary>
    public class WebMailService : IWebMailService
    {
        private readonly IViewRendererService _viewRendererService;

        /// <summary>
        /// A web mail service using the `MailKit` library.
        /// </summary>
        public WebMailService(IViewRendererService viewRendererService)
        {
            _viewRendererService = viewRendererService ?? throw new ArgumentNullException(nameof(viewRendererService));
        }

        /// <summary>
        /// Sends an email using the `MailKit` library.
        /// This method converts a razor template file to an string and then uses it as the email's message.
        /// </summary>
        public async Task SendEmailAsync<T>(
            SmtpConfig smtpConfig,
            IEnumerable<MailAddress> emails,
            string subject,
            string viewNameOrPath,
            T viewModel,
            IEnumerable<MailAddress> blindCarpbonCopies = null,
            IEnumerable<MailAddress> carpbonCopies = null,
            IEnumerable<MailAddress> replyTos = null,
            DelayDelivery delayDelivery = null,
            IEnumerable<string> attachmentFiles = null,
            MailHeaders headers = null)
        {
            var message = await _viewRendererService.RenderViewToStringAsync(viewNameOrPath, viewModel);
            await SendEmailAsync(smtpConfig, emails, subject, message,
              blindCarpbonCopies, carpbonCopies, replyTos, delayDelivery, attachmentFiles, headers);
        }

        /// <summary>
        /// Sends an email using the `MailKit` library.
        /// </summary>
        public async Task SendEmailAsync(
            SmtpConfig smtpConfig,
            IEnumerable<MailAddress> emails,
            string subject,
            string message,
            IEnumerable<MailAddress> blindCarpbonCopies = null,
            IEnumerable<MailAddress> carpbonCopies = null,
            IEnumerable<MailAddress> replyTos = null,
            DelayDelivery delayDelivery = null,
            IEnumerable<string> attachmentFiles = null,
            MailHeaders headers = null)
        {
            if (smtpConfig.UsePickupFolder)
            {
                const int maxBufferSize = 0x10000; // 64K.

                if (!Directory.Exists(smtpConfig.PickupFolder))
                {
                    Directory.CreateDirectory(smtpConfig.PickupFolder);
                }

                foreach (var email in emails)
                {
                    using (var stream = new FileStream(
                        Path.Combine(smtpConfig.PickupFolder, $"email-{Guid.NewGuid().ToString("N")}.eml"),
                        FileMode.CreateNew, FileAccess.Write, FileShare.None,
                        maxBufferSize, useAsync: true))
                    {
                        var emailMessage = getEmailMessage(email.ToName, email.ToAddress, subject,
                        message, attachmentFiles, smtpConfig, headers, blindCarpbonCopies, carpbonCopies, replyTos);
                        await emailMessage.WriteToAsync(stream);
                    }
                }
            }
            else
            {
                using (var client = new SmtpClient())
                {
                    if (!string.IsNullOrWhiteSpace(smtpConfig.LocalDomain))
                    {
                        client.LocalDomain = smtpConfig.LocalDomain;
                    }
                    await client.ConnectAsync(smtpConfig.Server, smtpConfig.Port, SecureSocketOptions.Auto);
                    if (!string.IsNullOrWhiteSpace(smtpConfig.Username) &&
                        !string.IsNullOrWhiteSpace(smtpConfig.Password))
                    {
                        await client.AuthenticateAsync(smtpConfig.Username, smtpConfig.Password);
                    }

                    var count = 0;
                    foreach (var email in emails)
                    {
                        var emailMessage = getEmailMessage(email.ToName, email.ToAddress, subject,
                        message, attachmentFiles, smtpConfig, headers, blindCarpbonCopies, carpbonCopies, replyTos);
                        await client.SendAsync(emailMessage);
                        count++;

                        if (delayDelivery != null)
                        {
                            if (count % delayDelivery.NumberOfMessages == 0)
                            {
                                await Task.Delay(delayDelivery.Delay);
                            }
                        }
                    }

                    await client.DisconnectAsync(true);
                }
            }
        }

        private static MimeMessage getEmailMessage(
            string toName, string toAddress, string subject, string message,
            IEnumerable<string> attachmentFiles, SmtpConfig smtpConfig, MailHeaders headers,
            IEnumerable<MailAddress> blindCarpbonCopies, IEnumerable<MailAddress> carpbonCopies,
            IEnumerable<MailAddress> replyTos)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(smtpConfig.FromName, smtpConfig.FromAddress));
            emailMessage.Subject = subject.ApplyRle();
            emailMessage.To.Add(new MailboxAddress(toName ?? string.Empty, toAddress));

            if (blindCarpbonCopies != null && blindCarpbonCopies.Any())
            {
                foreach (var bcc in blindCarpbonCopies)
                {
                    emailMessage.Bcc.Add(new MailboxAddress(bcc.ToName ?? string.Empty, bcc.ToAddress));
                }
            }

            if (carpbonCopies != null && carpbonCopies.Any())
            {
                foreach (var cc in carpbonCopies)
                {
                    emailMessage.Cc.Add(new MailboxAddress(cc.ToName ?? string.Empty, cc.ToAddress));
                }
            }

            if (replyTos != null && replyTos.Any())
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

        private static void addHeaders(MimeMessage emailMessage, MailHeaders headers, string fromAddress)
        {
            if (headers == null)
            {
                return;
            }

            var host = fromAddress.Split('@').Last();

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

        private static MimeEntity getMessageBody(string message, IEnumerable<string> attachmentFiles)
        {
            var builder = new BodyBuilder();
            builder.HtmlBody = message;
            if (attachmentFiles != null && attachmentFiles.Any())
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
}