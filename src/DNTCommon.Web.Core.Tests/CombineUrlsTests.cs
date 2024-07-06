using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class CombineUrlsTests
{
    [TestMethod]
    public void CombineUrlShouldWorkWithRelativeUrls()
    {
        var result = "/site".CombineUrl(relativeUrl: "/path1").CombineUrl(relativeUrl: "/path2");
        Assert.AreEqual(expected: "/site/path1/path2", result);
    }

    [TestMethod]
    public void CombineUrlShouldWorkWithSimpleRelativeUrls()
    {
        var result = "/site".CombineUrl(relativeUrl: "path1").CombineUrl(relativeUrl: "path2");
        Assert.AreEqual(expected: "/site/path1/path2", result);
    }

    [TestMethod]
    public void CombineUrlShouldWorkWithAbsoluteUrls()
    {
        var result = "https://site.com".CombineUrl(relativeUrl: "/path1").CombineUrl(relativeUrl: "/path2");
        Assert.AreEqual(expected: "https://site.com/path1/path2", result);
    }

    [TestMethod]
    public void CombineUrlsShouldWorkWithRelativeUrls()
    {
        var result = "/site".CombineUrls("/path1", "/path2");
        Assert.AreEqual(expected: "/site/path1/path2", result);
    }

    [TestMethod]
    public void CombineUrlsShouldWorkWithAbsoluteUrls()
    {
        var result = "https://site.com".CombineUrls("/path1", "/path2");
        Assert.AreEqual(expected: "https://site.com/path1/path2", result);
    }
}