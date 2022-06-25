namespace DNTCommon.Web.Core;

/// <summary>
/// The SMTP server's config
/// </summary>
public class SmtpConfig
{
    /// <summary>
    /// The host name to connect to.
    /// </summary>
    public string Server { get; set; } = default!;

    /// <summary>
    /// The optional user name.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// The optional password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// The port to connect to. If the specified port is 0, then the default port will be used.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// The local domain is used in the HELO or EHLO commands sent to the SMTP server. If left unset, the local IP address will be used instead.
    /// </summary>
    public string? LocalDomain { get; set; }

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
    public string? PickupFolder { get; set; }

    /// <summary>
    /// The name of the mailbox.
    /// </summary>
    public string FromName { get; set; } = default!;

    /// <summary>
    /// The address of the mailbox.
    /// </summary>
    public string FromAddress { get; set; } = default!;
}