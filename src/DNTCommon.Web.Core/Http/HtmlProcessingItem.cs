using HtmlAgilityPack;

namespace DNTCommon.Web.Core;

public record HtmlProcessingItem(HtmlDocument HtmlDocument, HtmlNode Node, HtmlAttribute Attribute);
