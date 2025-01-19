using MimeKit;

namespace DNTCommon.Web.Core;

/// <summary>
///     Url Validators
/// </summary>
public static class UrlValidatorExtensions
{
    /// <summary>
    ///     Tries to create a new Uri using the specified String instance and a UriKind. Returns true if the Uri was
    ///     successfully created; otherwise, false.
    /// </summary>
    public static bool IsValidUrl([NotNullWhen(returnValue: true)] this string? url, UriKind uriKind = UriKind.Absolute)
        => !string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, uriKind, out var uri) &&
           (string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    ///     Tries to parse the given text into a new MailboxAddress instance. Returns true, if the address was successfully
    ///     parsed, false otherwise.
    /// </summary>
    public static bool IsValidEmail([NotNullWhen(returnValue: true)] this string? to)
        => !string.IsNullOrWhiteSpace(to) && MailboxAddress.TryParse(ParserOptions.Default, to, out _);

    /// <summary>
    ///     This will normalize the gmail and facebook emails
    /// </summary>
    public static string FixGmailDots(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return email;
        }

        email = email.ToLowerInvariant().Trim();
        var emailParts = email.Split(separator: '@', StringSplitOptions.RemoveEmptyEntries);

        if (emailParts.Length == 1)
        {
            return email;
        }

        var name = emailParts[0].Replace(oldValue: ".", string.Empty, StringComparison.OrdinalIgnoreCase);

        var plusIndex = name.IndexOf(value: '+', StringComparison.OrdinalIgnoreCase);

        if (plusIndex != -1)
        {
            name = name[..plusIndex];
        }

        var emailDomain = emailParts[1];

        emailDomain = emailDomain.Replace(oldValue: "googlemail.com", newValue: "gmail.com",
            StringComparison.OrdinalIgnoreCase);

        string[] domainsAllowedDots = ["gmail.com", "facebook.com"];

        var isFromDomainsAllowedDots =
            domainsAllowedDots.Any(domain => emailDomain.Equals(domain, StringComparison.OrdinalIgnoreCase));

        return !isFromDomainsAllowedDots
            ? email
            : string.Format(CultureInfo.InvariantCulture, format: "{0}@{1}", name, emailDomain);
    }
}