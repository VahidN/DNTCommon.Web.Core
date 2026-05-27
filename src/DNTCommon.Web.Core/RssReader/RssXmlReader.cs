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

        var parsed = raw.TryParseFeedDate();

        if (parsed.HasValue)
        {
            //  نکته کلیدی: تبدیل به RFC1123 برای Syndication
            return parsed.Value.ToUniversalTime().ToString(format: "R", CultureInfo.InvariantCulture);
        }

        // fallback: مقدار اصلی (Syndication ممکن است default بگذارد ولی crash نمی‌کند)
        return raw;
    }
}
