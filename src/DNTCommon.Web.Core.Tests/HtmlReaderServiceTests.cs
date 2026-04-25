using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class HtmlReaderServiceTests : TestsBase
{
    [TestMethod]
    public void TestExtractImagesLinksReturnsAllImages()
    {
        var items = HtmlHelperServiceTests.Html.GetHtmlNodesByName(nodeName: "img").ToList();
        Assert.AreEqual(expected: 8, items.Count);
    }

    [TestMethod]
    public void TestExtractLinksReturnsAllLinks()
    {
        var items = HtmlHelperServiceTests.Html.GetHtmlNodesByName(nodeName: "a").ToList();
        Assert.AreEqual(expected: 4, items.Count);
    }

    [TestMethod]
    public void TestExtractBodyReturnsOne()
    {
        var items = HtmlHelperServiceTests.Html.GetHtmlNodesByName(nodeName: "body").ToList();
        Assert.AreEqual(expected: 1, items.Count);
    }
}
