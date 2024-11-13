using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core;

/// <summary>
///     An ASP.NET Core OpenSearch provider.
/// </summary>
public class OpenSearchResult : ActionResult
{
    /// <summary>
    ///     A short name for the search engine. The value must contain 16 or fewer characters of plain text.
    ///     The value must not contain HTML or other markup.
    /// </summary>
    public string ShortName { set; get; } = default!;

    /// <summary>
    ///     A brief description of the search engine. The value must contain 1024 or fewer characters of plain text.
    ///     The value must not contain HTML or other markup.
    /// </summary>
    public string Description { set; get; } = default!;

    /// <summary>
    ///     The URL to go to to open up the search page at the site for which the plugin is designed to search.
    ///     This provides a way for Firefox to let the user visit the web site directly.
    /// </summary>
    public string SearchForm { set; get; } = default!;

    /// <summary>
    ///     URI to an icon representative of the search engine. When possible,
    ///     search engines should offer a 16x16 image of type "image/x-icon" and a 64x64 image of type "image/jpeg"
    ///     or "image/png".
    /// </summary>
    public string FavIconUrl { set; get; } = default!;

    /// <summary>
    ///     Describes the URL or URLs to use for the search. The method attribute indicates whether to use a
    ///     GET or POST request to fetch the result. The template attribute indicates the base URL for the search query.
    /// </summary>
    public string SearchUrlTemplate { set; get; } = default!;

    /// <summary>
    ///     Executes the result operation of the action method synchronously.
    /// </summary>
    public override Task ExecuteResultAsync(ActionContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var response = context.HttpContext.Response;

        var mediaType = new MediaTypeHeaderValue(mediaType: "application/opensearchdescription+xml")
        {
            CharSet = Encoding.UTF8.WebName
        };

        response.ContentType = mediaType.ToString();

        var data = GetOpenSearchData();

        return response.Body.WriteAsync(data.AsMemory(start: 0, data.Length)).AsTask();
    }

    private byte[] GetOpenSearchData()
    {
        using var memoryStream = new MemoryStream();

        var xws = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8
        };

        using var xmlWriter = XmlWriter.Create(memoryStream, xws);

        xmlWriter.WriteStartElement(localName: "OpenSearchDescription", ns: "http://a9.com/-/spec/opensearch/1.1/");

        xmlWriter.WriteElementString(localName: "ShortName", ShortName);
        xmlWriter.WriteElementString(localName: "Description", Description);
        xmlWriter.WriteElementString(localName: "InputEncoding", value: "UTF-8");
        xmlWriter.WriteElementString(localName: "SearchForm", SearchForm);

        xmlWriter.WriteStartElement(localName: "Url");
        xmlWriter.WriteAttributeString(localName: "type", value: "text/html");
        xmlWriter.WriteAttributeString(localName: "template", SearchUrlTemplate);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement(localName: "Image");
        xmlWriter.WriteAttributeString(localName: "width", value: "16");
        xmlWriter.WriteAttributeString(localName: "height", value: "16");
        xmlWriter.WriteString(FavIconUrl);
        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndElement();

        return memoryStream.ToArray();
    }
}