using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class HtmlReaderServiceTests : TestsBase
{
    [TestMethod]
    public void TestExtractImagesLinksReturnsAllImages()
    {
        ServiceProvider.RunScopedService<IHtmlReaderService>(htmlReaderService =>
        {
            var items = htmlReaderService.ParseHtml(HtmlHelperServiceTests.Html).HtmlNodes.ToList();
            Assert.AreEqual(expected: 8, actual: items.Count(node => node.Name == "img"));
        });
    }

    [TestMethod]
    public void TestExtractLinksReturnsAllLinks()
    {
        ServiceProvider.RunScopedService<IHtmlReaderService>(htmlReaderService =>
        {
            var items = htmlReaderService.ParseHtml(HtmlHelperServiceTests.Html).HtmlNodes.ToList();
            Assert.AreEqual(expected: 4, actual: items.Count(node => node.Name == "a"));
        });
    }
}