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
}