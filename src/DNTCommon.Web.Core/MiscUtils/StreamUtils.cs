using System.Runtime.InteropServices;
using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Stream helpers
/// </summary>
public static class StreamUtils
{
    /// <summary>
    ///     Reads all characters from the current stream from the beginning to the end
    ///     and returns them as a single string.
    /// </summary>
    /// <param name="stream">The input stream. It cannot be null.</param>
    /// <param name="readChunkBufferLength">
    ///     The size of the buffer. The default size is 4096.
    /// </param>
    /// <returns>
    ///     The rest of the stream as a string, from the beginning to the end.
    ///     Or null if the stream is a null reference or not readable.
    /// </returns>
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
    /// Determines whether the specified stream is readable and seekable.
    /// </summary>
    /// <param name="stream">The stream to check.</param>
    /// <returns>true if the stream is readable and seekable; otherwise, false.</returns>
    public static bool IsReadableStream([NotNullWhen(returnValue: true)] this Stream? stream)
        => stream is { CanRead: true, CanSeek: true };

    /// <summary>
    ///     Reads the stream into a byte array.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="readChunkBufferLength">Size of the read buffer.</param>
    /// <returns>
    ///     A byte array containing the data from the stream, or null if the stream is null or not readable.
    /// </returns>
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
    /// Tries to read the first N bytes from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="numberOfBytes">The number of bytes to read.</param>
    /// <returns>
    /// A byte array containing the first N bytes of the stream,
    /// or null if the stream is null, not readable, or empty.
    /// </returns>
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
    /// Tries to read the first N bytes from a file.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="numberOfBytes">The number of bytes to read.</param>
    /// <returns>
    /// A byte array containing the first N bytes of the file,
    /// or null if the file does not exist or an error occurs.
    /// </returns>
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
    ///     Converts the string to a byte array using the specified encoding.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <param name="inputEncoding">The encoding to use. The default value is UTF8.</param>
    /// <returns>
    ///     The byte array representation of the string, or <see langword="null"/> if the input string is <see langword="null"/> or empty.
    /// </returns>
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
    /// Converts a string to a read-only byte span using the Unicode encoding.
    /// </summary>
    /// <param name="text">The string to convert. If null or empty, null is returned.</param>
    /// <returns>A read-only byte span representing the string, or null if the input string is null or empty.</returns>
    public static ReadOnlySpan<byte> ToByteSpan([NotNullIfNotNull(nameof(text))] this string? text)
        => text.IsEmpty() ? null : MemoryMarshal.Cast<char, byte>(text);
}
