using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class CollectionsExtensionsTests : TestsBase
{
    [TestMethod]
    [DataRow(new[]
    {
        "12", "13", "14"
    }, new[]
    {
        "12", "13"
    })]
    public void TestStartsWith(string[] source, string[] destination) => Assert.IsTrue(source.StartsWith(destination));

    [TestMethod]
    [DataRow(new[]
    {
        "12", "13", "14"
    }, new[]
    {
        "13", "14"
    })]
    public void TestEndsWith(string[] source, string[] destination) => Assert.IsTrue(source.EndsWith(destination));

    [TestMethod]
    [DataRow(new[]
    {
        "12", "13", "14"
    }, new[]
    {
        "12", "14"
    })]
    public void TestContainsNonSequentially(string[] source, string[] destination)
        => Assert.IsTrue(source.ContainsNonSequentially(destination));

    [TestMethod]
    [DataRow(new[]
    {
        "12", "13", "14"
    }, new[]
    {
        "12", "14"
    })]
    public void TestContainsSequentially(string[] source, string[] destination)
        => Assert.IsFalse(source.ContainsSequentially(destination));
}