using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class EnumExtensionsTests
{
    [TestMethod]
    public void TestGetRandomEnumItemWorks()
    {
        var result = TestItems.GetRandomItem();
        Assert.IsTrue(TestItems.GetValues().HasAny(item => item == result));
    }
}

public enum TestItems
{
    Item1,
    Item2,
    Item3
}
