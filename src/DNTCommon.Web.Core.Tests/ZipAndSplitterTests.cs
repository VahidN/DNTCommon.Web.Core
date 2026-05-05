using System.Threading.Tasks;
using DNTCommon.Web.Core;
using DNTCommon.Web.Core.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ZipAndSplitterTests : TestsBase
{
    [TestMethod]
    public async Task TestZipAndSplitFileToMultiplePartsAsync()
    {
        var file = @"C:\file.zip";
        var temp = @"C:\bin";

        var parts = await file.ZipAndSplitFileToMultiplePartsAsync(temp, partSizeMB: 1);
        Assert.IsTrue(parts?.Count > 0);
    }

    [TestMethod]
    public async Task TestZipAndSplitFolderToMultiplePartsAsync()
    {
        var folder = @"C:\wwwroot";
        var temp = @"C:\bin";

        var parts = await folder.ZipAndSplitFolderToMultiplePartsAsync(temp, partSizeMB: 1);
        Assert.IsTrue(parts?.Count > 0);
    }
}
