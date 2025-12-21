using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class PathUtilsTests
{
    [TestMethod]
    [DataRow(@"C:\App\Data\", "/test", "test")]
    [DataRow(@"C:\App\Data\", "test", "test")]
    [DataRow(@"C:\App\Data\", "images/logo.jpg", @"images\logo.jpg")]
    [DataRow(@"C:\App\Data\", "images/folder/logo.jpg", @"images\folder\logo.jpg")]
    public void TestGetSanitizedRelativePathWorks(string root, string data, string expected)
        => Assert.AreEqual(expected, root.GetSanitizedRelativePath(data));

    [TestMethod]
    [DataRow(@"C:\App\Data\", "../../../etc/passwd")]
    [DataRow(@"C:\App\Data\", "C:\\Windows\\System32.dll")]
    public void TestGetSanitizedRelativePathWithInvalidInputsWorks(string root, string data)
        => Assert.ThrowsExactly<UnauthorizedAccessException>(() => root.GetSanitizedRelativePath(data));

    [TestMethod]
    [DataRow(@"C:\App\Data\", "/test", @"C:\App\Data\test")]
    [DataRow(@"C:\App\Data\", "test", @"C:\App\Data\test")]
    [DataRow(@"C:\App\Data\", "/test/", @"C:\App\Data\test")]
    [DataRow(@"C:\App\Data\", "logo.jpg", @"C:\App\Data\logo.jpg")]
    [DataRow(@"C:\App\Data\", "images/logo.jpg", @"C:\App\Data\images\logo.jpg")]
    [DataRow(@"C:\App\Data\", "/images/logo.jpg/", @"C:\App\Data\images\logo.jpg")]
    [DataRow(@"C:\App\Data\", "images/folder/logo.jpg", @"C:\App\Data\images\folder\logo.jpg")]
    [DataRow(@"C:\App\Data\", "images/folder/logo.jpg/", @"C:\App\Data\images\folder\logo.jpg")]
    public void TestSafePathCombineWorks(string root, string data, string expected)
        => Assert.AreEqual(expected, root.SafePathCombine(data));

    [TestMethod]
    [DataRow(@"C:\App\Data\", "/test", "logo1.jpg", @"C:\App\Data\test\logo1.jpg")]
    [DataRow(@"C:\App\Data\", "test", "logo1.jpg", @"C:\App\Data\test\logo1.jpg")]
    [DataRow(@"C:\App\Data\", "/test/", "logo1.jpg", @"C:\App\Data\test\logo1.jpg")]
    [DataRow(@"C:\App\Data\", "logo.jpg", "logo1.jpg", @"C:\App\Data\logo.jpg\logo1.jpg")]
    [DataRow(@"C:\App\Data\", "images/logo.jpg", "logo1.jpg", @"C:\App\Data\images\logo.jpg\logo1.jpg")]
    [DataRow(@"C:\App\Data\", "/images/logo.jpg/", "logo1.jpg", @"C:\App\Data\images\logo.jpg\logo1.jpg")]
    [DataRow(@"C:\App\Data\", "images/folder/logo.jpg", "logo1.jpg", @"C:\App\Data\images\folder\logo.jpg\logo1.jpg")]
    [DataRow(@"C:\App\Data\", "images/folder/logo.jpg/", "logo1.jpg", @"C:\App\Data\images\folder\logo.jpg\logo1.jpg")]
    public void TestSafePathCombineWithMultipleInputsWorks(string root, string data1, string data2, string expected)
        => Assert.AreEqual(expected, root.SafePathCombine(data1, data2));
}
