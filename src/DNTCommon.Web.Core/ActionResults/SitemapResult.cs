using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     Represents a sitemap result that can be returned from an ASP.NET Core controller action.
///     It generates an XML sitemap based on the provided <see cref="SitemapItem" /> list.
/// </summary>
public class SitemapResult(IList<SitemapItem> allItems) : ActionResult
{
    /// <summary>
    ///     Executes the result operation of the action method asynchronously.
    /// </summary>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var httpContextInfo = context.HttpContext.RequestServices.GetRequiredService<IHttpRequestInfoService>();
        var response = context.HttpContext.Response;

        var mediaType = new MediaTypeHeaderValue(mediaType: "application/xml")
        {
            CharSet = Encoding.UTF8.WebName
        };

        response.ContentType = mediaType.ToString();

        var baseUrl = httpContextInfo.GetBaseUrl();

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(message: "baseUrl is null.");
        }

        var data = GetSitemapData(baseUrl);

        return response.Body.WriteAsync(data.AsMemory(start: 0, data.Length)).AsTask();
    }

    private byte[] GetSitemapData(string baseUrl)
    {
        using var memoryStream = new MemoryStream();

        var xws = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8
        };

        using var xmlWriter = XmlWriter.Create(memoryStream, xws);

        xmlWriter.WriteStartElement(localName: "urlset", ns: "https://www.sitemaps.org/schemas/sitemap/0.9");
        xmlWriter.WriteStartElement(localName: "url");
        xmlWriter.WriteElementString(localName: "loc", baseUrl);

        xmlWriter.WriteElementString(localName: "lastmod",
            DateTime.UtcNow.ToString(format: "yyyy-MM-dd", CultureInfo.InvariantCulture));

        xmlWriter.WriteElementString(localName: "changefreq", value: "daily");
        xmlWriter.WriteElementString(localName: "priority", value: "1.0");
        xmlWriter.WriteEndElement();

        foreach (var item in allItems)
        {
            xmlWriter.WriteStartElement(localName: "url");
            xmlWriter.WriteElementString(localName: "loc", item.Url);

            xmlWriter.WriteElementString(localName: "lastmod",
                item.LastUpdatedTime.ToString(format: "yyyy-MM-dd", CultureInfo.InvariantCulture));

            xmlWriter.WriteElementString(localName: "changefreq", item.ChangeFrequency.ToString().ToLowerInvariant());

            xmlWriter.WriteElementString(localName: "priority", item.Priority.ToString(CultureInfo.InvariantCulture));

            xmlWriter.WriteEndElement();
        }

        xmlWriter.WriteEndElement();

        return memoryStream.ToArray();
    }
}
