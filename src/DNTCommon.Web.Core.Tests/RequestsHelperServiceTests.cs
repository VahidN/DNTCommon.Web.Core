using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class RequestsHelperServiceTests : TestsBase
{
    [DataTestMethod]
    [DataRow("https://www.dntips.ir",
        "https://www.dntips.ir/DntSite.Web.styles.css?v=VrLAx5GuTjkrBQODJwFQSbZD2VUeEfVdFP0F-eD8yg4",
        "https://www.dntips.ir/posts")]
    public void TestShouldSkipThisRequest(string rootUrl, string referrerUrl, string destinationUrl)
    {
        var isNullOrEmpty = string.IsNullOrEmpty(referrerUrl);
        var isNotReferrer = !destinationUrl.IsReferrerToThisSite(rootUrl);
        var isLocalReferrer = referrerUrl.IsLocalReferrer(destinationUrl);

        var shouldSkip = isNullOrEmpty || isNotReferrer || isLocalReferrer;

        Assert.IsTrue(shouldSkip);
    }

    [DataTestMethod]
    [DataRow("https://www.dntips.ir", "https://www.github.com/VahidN", "https://www.dntips.ir/posts")]
    public void TestShouldNotSkipThisRequest(string rootUrl, string referrerUrl, string destinationUrl)
    {
        var isNullOrEmpty = string.IsNullOrEmpty(referrerUrl);
        var isNotReferrer = !destinationUrl.IsReferrerToThisSite(rootUrl);
        var isLocalReferrer = referrerUrl.IsLocalReferrer(destinationUrl);

        var shouldSkip = isNullOrEmpty || isNotReferrer || isLocalReferrer;

        Assert.IsFalse(shouldSkip);
    }

    [TestMethod]
    public void TestUriHelperEncode()
    {
        var url = "/post/3406/مشكل همزمان";
        var encodeUrl = UriHelper.Encode(new Uri(url, UriKind.RelativeOrAbsolute));

        Assert.AreEqual(expected: "/post/3406/%D9%85%D8%B4%D9%83%D9%84%20%D9%87%D9%85%D8%B2%D9%85%D8%A7%D9%86",
            encodeUrl);
    }

    [TestMethod]
    public void TestRemoveQueryStrings()
        => Assert.AreEqual(expected: "https://www.dntips.ir/news/details/19377",
            "https://www.dntips.ir/news/details/19377?utm_source=feed&utm_medium=rss&utm_campaign=featured&utm_updated=1403-07-15-07-30"
                .GetUrlWithoutRssQueryStrings());
}