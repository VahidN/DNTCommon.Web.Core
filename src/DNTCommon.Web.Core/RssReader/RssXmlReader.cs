using System.Xml;

namespace DNTCommon.Web.Core;

/// <summary>
///     Based on
///     https://learn.microsoft.com/en-us/previous-versions/troubleshoot/dotnet/framework/rss20feedformatter-throw-exception
///     to fix System.ServiceModel.Syndication.SyndicationItem.get_PublishDate() errors
/// </summary>
/// <param name="reader"></param>
public sealed class RssXmlReader(TextReader reader) : XmlTextReader(reader)
{
    private static readonly string[] DateFormats =
    [
        // RFC 822 / 1123
        "r", "R", "ddd, dd MMM yyyy HH:mm:ss GMT", "ddd, dd MMM yyyy HH:mm:ss UTC", "ddd, dd MMM yyyy HH:mm:ss zzz",

        // ISO / Atom
        "yyyy-MM-dd'T'HH:mm:ssZ", "yyyy-MM-dd'T'HH:mm:sszzz", "yyyy-MM-dd'T'HH:mm:ss.fffZ",
        "yyyy-MM-dd'T'HH:mm:ss.fffzzz",

        // رایج ولی غیر استاندارد
        "yyyy-MM-dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy/MM/dd HH:mm",

        // عجیب ولی دیده‌شده
        "ddd MMM dd HH:mm:ss GMT yyyy", "ddd MMM dd HH:mm:ss zzz yyyy", "ddd MMM dd HH:mm:ss Z yyyy"
    ];

    private bool _readingDate;

    public override void ReadStartElement()
    {
        if (NamespaceURI?.Length == 0 && (LocalName.Equals(value: "pubDate", StringComparison.OrdinalIgnoreCase) ||
                                          LocalName.Equals(value: "lastBuildDate", StringComparison.OrdinalIgnoreCase)))
        {
            _readingDate = true;
        }

        base.ReadStartElement();
    }

    public override void ReadEndElement()
    {
        _readingDate = false;
        base.ReadEndElement();
    }

    public override string ReadString()
    {
        if (!_readingDate)
        {
            return base.ReadString();
        }

        var raw = base.ReadString().Trim();

        if (string.IsNullOrWhiteSpace(raw))
        {
            return raw;
        }

        if (TryParseDate(raw, out var parsed))
        {
            //  نکته کلیدی: تبدیل به RFC1123 برای Syndication
            return parsed.ToUniversalTime().ToString(format: "R", CultureInfo.InvariantCulture);
        }

        // fallback: مقدار اصلی (Syndication ممکن است default بگذارد ولی crash نمی‌کند)
        return raw;
    }

    private static bool TryParseDate(string value, out DateTimeOffset result)
    {
        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out result))
        {
            return true;
        }

        return DateTimeOffset.TryParseExact(value, DateFormats, CultureInfo.InvariantCulture,
            DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out result);
    }
}
