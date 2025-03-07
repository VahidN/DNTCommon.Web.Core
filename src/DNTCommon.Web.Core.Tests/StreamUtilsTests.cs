using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class StreamUtilsTests
{
    [TestMethod]
    public void ToTextNullStreamReturnsNull()
    {
        Stream? stream = null;
        Assert.IsNull(stream.ToText());
    }

    [TestMethod]
    public void ToTextEmptyStreamReturnsEmptyString()
    {
        using var stream = new MemoryStream();
        Assert.AreEqual(string.Empty, stream.ToText());
    }

    [TestMethod]
    public void ToTextNonEmptyStreamReturnsCorrectString()
    {
        var text = "Hello, world!";
        var bytes = Encoding.UTF8.GetBytes(text);
        using var stream = new MemoryStream(bytes);
        Assert.AreEqual(text, stream.ToText());
    }

    [TestMethod]
    public void IsReadableStreamNullStreamReturnsFalse()
    {
        Stream? stream = null;
        Assert.IsFalse(stream.IsReadableStream());
    }

    [TestMethod]
    public void IsReadableStreamReadableStreamReturnsTrue()
    {
        using var stream = new MemoryStream();
        Assert.IsTrue(stream.IsReadableStream());
    }

    [TestMethod]
    public void ToBytesNullStreamReturnsNull()
    {
        Stream? stream = null;
        Assert.IsNull(stream.ToBytes());
    }

    [TestMethod]
    public void ToBytesEmptyStreamReturnsEmptyByteArray()
    {
        using var stream = new MemoryStream();
        CollectionAssert.AreEqual(Array.Empty<byte>(), stream.ToBytes());
    }

    [TestMethod]
    public void ToBytesNonEmptyStreamReturnsCorrectByteArray()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(bytes);
        CollectionAssert.AreEqual(bytes, stream.ToBytes());
    }

    [TestMethod]
    public void TryTakeFirstBytesNullStreamReturnsNull()
    {
        Stream? stream = null;
        Assert.IsNull(stream.TryTakeFirstBytes(5));
    }

    [TestMethod]
    public void TryTakeFirstBytesEmptyStreamReturnsNull()
    {
        using var stream = new MemoryStream();
        Assert.IsNull(stream.TryTakeFirstBytes(5));
    }

    [TestMethod]
    public void TryTakeFirstBytesFewerBytesThanRequestedReturnsAllBytes()
    {
        var bytes = new byte[] { 1, 2, 3 };
        using var stream = new MemoryStream(bytes);
        CollectionAssert.AreEqual(bytes, stream.TryTakeFirstBytes(5));
    }

    [TestMethod]
    public void TryTakeFirstBytesMoreBytesThanRequestedReturnsRequestedBytes()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(bytes);
        var expected = new byte[] { 1, 2, 3 };
        CollectionAssert.AreEqual(expected, stream.TryTakeFirstBytes(3));
    }

    [TestMethod]
    public void TryTakeFirstBytesFileNotExistsReturnsNull()
    {
        Assert.IsNull("not_exists.txt".TryTakeFirstBytes(5));
    }

    [TestMethod]
    public void ToBytesStringReturnsCorrectByteArray()
    {
        var text = "test";
        var bytes = text.ToBytes();
        CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(text), bytes);
    }

    [TestMethod]
    public void ToBytesStringWithEncodingReturnsCorrectByteArray()
    {
        var text = "test";
        var bytes = text.ToBytes(Encoding.ASCII);
        CollectionAssert.AreEqual(Encoding.ASCII.GetBytes(text), bytes);
    }

    [TestMethod]
    public void ToByteSpanEmptyStringReturnsNull()
    {
        var text = string.Empty;
        var span = text.ToByteSpan();
        Assert.IsTrue(span.IsEmpty);
    }

    [TestMethod]
    public void ToByteSpanNullStringReturnsNull()
    {
        string text = null;
        var span = text.ToByteSpan();
        Assert.IsTrue(span.IsEmpty);
    }

    [TestMethod]
    public void ToByteSpanStringReturnsCorrectSpan()
    {
        var text = "test";
        var span = text.ToByteSpan();
        CollectionAssert.AreEqual(Encoding.Unicode.GetBytes(text), span.ToArray());
    }
}