using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// An ASP.NET Core OpenSearch provider.
    /// </summary>
    public class OpenSearchResult : ActionResult
    {
        /// <summary>
        /// A short name for the search engine. The value must contain 16 or fewer characters of plain text.
        /// The value must not contain HTML or other markup.
        /// </summary>
        public string ShortName { set; get; }

        /// <summary>
        /// A brief description of the search engine. The value must contain 1024 or fewer characters of plain text.
        /// The value must not contain HTML or other markup.
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        ///	The URL to go to to open up the search page at the site for which the plugin is designed to search.
        /// This provides a way for Firefox to let the user visit the web site directly.
        /// </summary>
        public string SearchForm { set; get; }

        /// <summary>
        /// URI to an icon representative of the search engine. When possible,
        /// search engines should offer a 16x16 image of type "image/x-icon" and a 64x64 image of type "image/jpeg"
        /// or "image/png".
        /// </summary>
        public string FavIconUrl { set; get; }

        /// <summary>
        /// Describes the URL or URLs to use for the search. The method attribute indicates whether to use a
        /// GET or POST request to fetch the result. The template attribute indicates the base URL for the search query.
        /// </summary>
        public string SearchUrlTemplate { set; get; }

        /// <summary>
        /// Executes the result operation of the action method synchronously.
        /// </summary>
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            var mediaType = new MediaTypeHeaderValue("application/opensearchdescription+xml")
            {
                CharSet = Encoding.UTF8.WebName
            };
            response.ContentType = mediaType.ToString();

            var data = getOpenSearchData();
            await response.Body.WriteAsync(data, 0, data.Length);
        }

        private byte[] getOpenSearchData()
        {
            using (var memoryStream = new MemoryStream())
            {
                var xws = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 };
                using (var xmlWriter = XmlWriter.Create(memoryStream, xws))
                {
                    xmlWriter.WriteStartElement("OpenSearchDescription", "http://a9.com/-/spec/opensearch/1.1/");

                    xmlWriter.WriteElementString("ShortName", ShortName);
                    xmlWriter.WriteElementString("Description", Description);
                    xmlWriter.WriteElementString("InputEncoding", "UTF-8");
                    xmlWriter.WriteElementString("SearchForm", SearchForm);

                    xmlWriter.WriteStartElement("Url");
                    xmlWriter.WriteAttributeString("type", "text/html");
                    xmlWriter.WriteAttributeString("template", SearchUrlTemplate);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("Image");
                    xmlWriter.WriteAttributeString("width", "16");
                    xmlWriter.WriteAttributeString("height", "16");
                    xmlWriter.WriteString(FavIconUrl);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                }
                return memoryStream.ToArray();
            }
        }
    }
}