using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class CombineUrlsTests
{
    [TestMethod]
    public void CombineUrlShouldWorkWithRootUrls()
    {
        var result = "".CombineUrl(relativeUrl: "/path1", escapeRelativeUrl: false)
            .CombineUrl(relativeUrl: "/path2", escapeRelativeUrl: false);

        Assert.AreEqual(expected: "/path1/path2", result);
    }

    [TestMethod]
    public void CombineUrlShouldWorkWithRelativeUrls()
    {
        var result = "/site".CombineUrl(relativeUrl: "/path1", escapeRelativeUrl: false)
            .CombineUrl(relativeUrl: "/path2", escapeRelativeUrl: false);

        Assert.AreEqual(expected: "/site/path1/path2", result);
    }

    [TestMethod]
    public void CombineUrlShouldWorkWithSimpleRelativeUrls()
    {
        var result = "/site".CombineUrl(relativeUrl: "path1", escapeRelativeUrl: false)
            .CombineUrl(relativeUrl: "path2", escapeRelativeUrl: false);

        Assert.AreEqual(expected: "/site/path1/path2", result);
    }

    [TestMethod]
    public void CombineUrlShouldWorkWithAbsoluteUrls()
    {
        var result = "https://site.com".CombineUrl(relativeUrl: "/path1", escapeRelativeUrl: false)
            .CombineUrl(relativeUrl: "/path2", escapeRelativeUrl: false);

        Assert.AreEqual(expected: "https://site.com/path1/path2", result);
    }

    [TestMethod]
    public void CombineUrlsShouldWorkWithRelativeUrls()
    {
        var result = "/site".CombineUrls(escapeRelativeUrl: false, "/path1", "/path2");
        Assert.AreEqual(expected: "/site/path1/path2", result);
    }

    [TestMethod]
    public void CombineUrlsShouldWorkWithAbsoluteUrls()
    {
        var result = "https://site.com".CombineUrls(escapeRelativeUrl: false, "/path1", "/path2");
        Assert.AreEqual(expected: "https://site.com/path1/path2", result);
    }
}