using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class MimeTypesExtensionsTests
{
    [TestMethod]
    [DataRow(data: "test.png")]
    [DataRow(data: ".png")]
    [DataRow(data: "c:\\path\\path2\\file.png")]
    public void GetMimeTypeWorksWithGivenInputs(string input)
        => Assert.AreEqual(input.GetMimeType(), actual: "image/png");
}
