using System.Runtime.InteropServices;
using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Stream helpers
/// </summary>
public static class StreamUtils
{
    /// <summary>
    ///     Reads the given stream in chunks
    /// </summary>
    public static string? ToText([NotNullIfNotNull(nameof(stream))] this Stream? stream,
        int readChunkBufferLength = 4096)
    {
        if (stream?.IsReadableStream() != true)
        {
            return null;
        }

        if (stream.Position != 0)
        {
            stream.Seek(offset: 0, SeekOrigin.Begin);
        }

        using var textWriter = new StringWriter(CultureInfo.InvariantCulture);
        using var reader = new StreamReader(stream);

        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;

        do
        {
            readChunkLength = reader.ReadBlock(readChunk, index: 0, readChunkBufferLength);
            textWriter.Write(readChunk, index: 0, readChunkLength);
        }
        while (readChunkLength > 0);

        return textWriter.ToString();
    }

    /// <summary>
    ///     gets a value indicating whether the current stream supports reading.
    /// </summary>
    public static bool IsReadableStream([NotNullWhen(returnValue: true)] this Stream? stream)
        => stream is { CanRead: true, CanSeek: true };

    /// <summary>
    ///     Reads the given stream in chunks
    /// </summary>
    public static byte[]? ToBytes([NotNullIfNotNull(nameof(stream))] this Stream? stream,
        int readChunkBufferLength = 4096)
    {
        if (stream?.IsReadableStream() != true)
        {
            return null;
        }

        if (stream.Position != 0)
        {
            stream.Seek(offset: 0, SeekOrigin.Begin);
        }

        var capacity = stream.CanSeek ? (int)stream.Length : 0;
        using var output = new MemoryStream(capacity);

        int readLength;
        var buffer = new byte[readChunkBufferLength];

        do
        {
            readLength = stream.Read(buffer, offset: 0, buffer.Length);
            output.Write(buffer, offset: 0, readLength);
        }
        while (readLength > 0);

        return output.ToArray();
    }

    /// <summary>
    ///     Tries to read the n first bytes of the given stream.
    /// </summary>
    public static byte[]? TryTakeFirstBytes(this Stream? stream, int numberOfBytes)
    {
        if (stream?.IsReadableStream() != true)
        {
            return null;
        }

        if (stream.Position != 0)
        {
            stream.Seek(offset: 0, SeekOrigin.Begin);
        }

        var buffer = new byte[numberOfBytes];
        var readLength = stream.Read(buffer, offset: 0, buffer.Length);

        if (readLength <= 0)
        {
            return null;
        }

        using var output = new MemoryStream();
        output.Write(buffer, offset: 0, readLength);

        return output.ToArray();
    }

    /// <summary>
    ///     Tries to read the n first bytes of the given file.
    /// </summary>
    public static byte[]? TryTakeFirstBytes(this string? filePath, int numberOfBytes)
    {
        if (!filePath.FileExists())
        {
            return null;
        }

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return fs.TryTakeFirstBytes(numberOfBytes);
    }

    /// <summary>
    ///     Encodes all the characters in the specified string into a sequence of bytes.
    /// </summary>
    /// <param name="text">The string containing the characters to encode.</param>
    /// <param name="inputEncoding">
    ///     Its default value is `Encoding.UTF8`.
    /// </param>
    /// <returns>A byte array containing the results of encoding the specified set of characters.</returns>
    public static byte[]? ToBytes([NotNullIfNotNull(nameof(text))] this string? text, Encoding? inputEncoding = null)
    {
        if (text.IsEmpty())
        {
            return null;
        }

        inputEncoding ??= Encoding.UTF8;

        return inputEncoding.GetBytes(text);
    }

    /// <summary>
    ///     Casts a read-only span of one primitive type to a read-only span of another primitive type.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static ReadOnlySpan<byte> ToByteSpan([NotNullIfNotNull(nameof(text))] this string? text)
        => text.IsEmpty() ? null : MemoryMarshal.Cast<char, byte>(text);
}
