using System.Runtime.InteropServices;
using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Stream helpers
/// </summary>
public static class StreamUtils
{
    private const int
        MaxBufferSize =
            0x10000; // 64K. The artificial constraint due to win32 api limitations. Increasing the buffer size beyond 64k will not help in any circumstance, as the underlying SMB protocol does not support buffer lengths beyond 64k.

    /// <summary>
    ///     Reads all characters from the current stream from the beginning to the end
    ///     and returns them as a single string.
    /// </summary>
    /// <param name="stream">The input stream. It cannot be null.</param>
    /// <param name="offset">A byte offset relative to the origin parameter.</param>
    /// <param name="readChunkBufferLength">
    ///     The size of the buffer. The default size is 4096.
    /// </param>
    /// <param name="encoding">The character encoding to use. Its default value is Encoding.UTF8</param>
    /// <returns>
    ///     The rest of the stream as a string, from the beginning to the end.
    ///     Or null if the stream is a null reference or not readable.
    /// </returns>
    public static string? ToText([NotNullIfNotNull(nameof(stream))] this Stream? stream,
        int offset = 0,
        int readChunkBufferLength = 4096,
        Encoding? encoding = null)
    {
        if (stream?.IsReadableStream() != true)
        {
            return null;
        }

        if (stream.Position != offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
        }

        using var textWriter = new StringWriter(CultureInfo.InvariantCulture);
        using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);

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
    ///     Determines whether the specified stream is readable and seekable.
    /// </summary>
    /// <param name="stream">The stream to check.</param>
    /// <returns>true if the stream is readable and seekable; otherwise, false.</returns>
    public static bool IsReadableStream([NotNullWhen(returnValue: true)] this Stream? stream)
        => stream is { CanRead: true, CanSeek: true };

    /// <summary>
    ///     Reads the stream into a byte array.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="offset">A byte offset relative to the origin parameter.</param>
    /// <param name="readChunkBufferLength">Size of the read buffer.</param>
    /// <returns>
    ///     A byte array containing the data from the stream, or null if the stream is null or not readable.
    /// </returns>
    public static byte[]? ToBytes([NotNullIfNotNull(nameof(stream))] this Stream? stream,
        int offset = 0,
        int readChunkBufferLength = 4096)
    {
        if (stream?.IsReadableStream() != true)
        {
            return null;
        }

        if (stream.Position != offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
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
    ///     Tries to read the first N bytes from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="numberOfBytes">The number of bytes to read.</param>
    /// <param name="offset">A byte offset relative to the origin parameter.</param>
    /// <returns>
    ///     A byte array containing the first N bytes of the stream,
    ///     or null if the stream is null, not readable, or empty.
    /// </returns>
    public static byte[]? TryTakeFirstBytes(this Stream? stream, int numberOfBytes, int offset = 0)
    {
        if (stream?.IsReadableStream() != true)
        {
            return null;
        }

        if (stream.Position != offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
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
    ///     Tries to read the first N bytes from a file.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="numberOfBytes">The number of bytes to read.</param>
    /// <param name="offset">A byte offset relative to the origin parameter.</param>
    /// <returns>
    ///     A byte array containing the first N bytes of the file,
    ///     or null if the file does not exist or an error occurs.
    /// </returns>
    public static byte[]? TryTakeFirstBytes(this string? filePath, int numberOfBytes, int offset = 0)
    {
        if (!filePath.FileExists())
        {
            return null;
        }

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return fs.TryTakeFirstBytes(numberOfBytes, offset);
    }

    /// <summary>
    ///     Initializes a new instance of the FileStream class with the specified path, creation mode, read/write and sharing
    ///     permission, and buffer size.
    /// </summary>
    public static FileStream? ToFileStream([NotNullIfNotNull(nameof(filePath))] this string? filePath,
        FileMode openOrCreateMode,
        FileAccess fileAccess,
        FileShare fileShare = FileShare.None,
        bool useAsync = false)
        => filePath.IsEmpty()
            ? null
            : new FileStream(filePath, openOrCreateMode, fileAccess, fileShare, MaxBufferSize, useAsync);

    /// <summary>
    ///     Converts the string to a byte array using the specified encoding.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <param name="inputEncoding">The encoding to use. The default value is UTF8.</param>
    /// <returns>
    ///     The byte array representation of the string, or <see langword="null" /> if the input string is
    ///     <see langword="null" /> or empty.
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
    ///     Converts the string to a MemoryStream using the specified encoding.
    /// </summary>
    /// <param name="text">The string to convert.</param>
    /// <param name="inputEncoding">The encoding to use. The default value is UTF8.</param>
    /// <returns>
    ///     The MemoryStream representation of the string, or <see langword="null" /> if the input string is
    ///     <see langword="null" /> or empty.
    /// </returns>
    public static MemoryStream? ToMemoryStream([NotNullIfNotNull(nameof(text))] this string? text,
        Encoding? inputEncoding = null)
    {
        if (text.IsEmpty())
        {
            return null;
        }

        inputEncoding ??= Encoding.UTF8;

        return new MemoryStream(inputEncoding.GetBytes(text));
    }

    /// <summary>
    ///     Converts a string to a read-only byte span using the Unicode encoding.
    /// </summary>
    /// <param name="text">The string to convert. If null or empty, null is returned.</param>
    /// <returns>A read-only byte span representing the string, or null if the input string is null or empty.</returns>
    public static ReadOnlySpan<byte> ToByteSpan([NotNullIfNotNull(nameof(text))] this string? text)
        => text.IsEmpty() ? null : MemoryMarshal.Cast<char, byte>(text);

    /// <summary>
    ///     Initializes a new instance of the FileStream class with the specified path, creation mode, read/ write and sharing
    ///     permission, buffer size, and synchronous or asynchronous state.
    /// </summary>
    public static FileStream CreateAsyncFileStream(this string path, FileMode openOrCreateMode, FileAccess fileAccess)
        => new(path, openOrCreateMode, fileAccess, FileShare.None, MaxBufferSize,

            // you have to explicitly open the FileStream as asynchronous
            // or else you're just doing synchronous operations on a background thread.
            useAsync: true);

    /// <summary>
    ///     Tries to find the encoding of the given file based on its BOM.
    /// </summary>
    public static Encoding? GetFileEncoding([NotNullIfNotNull(nameof(filePath))] this string? filePath)
    {
        if (filePath.IsEmpty())
        {
            return null;
        }

        var bom = filePath.TryTakeFirstBytes(numberOfBytes: 4);

        if (bom is null)
        {
            return null;
        }

#pragma warning disable SYSLIB0001
        if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
        {
            return Encoding.UTF7;
        }
#pragma warning restore SYSLIB0001

        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
        {
            return Encoding.UTF8;
        }

        if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0)
        {
            return Encoding.UTF32;
        }

        if (bom[0] == 0xff && bom[1] == 0xfe)
        {
            return Encoding.Unicode;
        }

        if (bom[0] == 0xfe && bom[1] == 0xff)
        {
            return Encoding.BigEndianUnicode;
        }

        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
        {
            return new UTF32Encoding(bigEndian: true, byteOrderMark: true);
        }

        return Encoding.ASCII;
    }

    /// <summary>
    ///     Convert Stream to Byte
    /// </summary>
    public static byte? TakeByte(this Stream? stream, long offset)
    {
        if (stream?.IsReadableStream() != true)
        {
            return null;
        }

        if (stream.Position != offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
        }

        return (byte)stream.ReadByte();
    }

    /// <summary>
    ///     Convert Stream to unsigned integer
    /// </summary>
    public static uint? TakeUInt(this Stream? stream, int offset)
    {
        if (stream?.IsReadableStream() != true)
        {
            return null;
        }

        if (stream.Position != offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
        }

        var bytes = stream.TryTakeFirstBytes(numberOfBytes: 4, offset);

        if (bytes is null)
        {
            return null;
        }

        return BitConverter.ToUInt32(bytes, startIndex: 0);
    }

    /// <summary>
    ///     Convert Stream to unsigned short
    /// </summary>
    public static ushort? TakeUShort(this Stream? stream, int offset)
    {
        if (stream?.IsReadableStream() != true)
        {
            return null;
        }

        if (stream.Position != offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
        }

        var bytes = stream.TryTakeFirstBytes(numberOfBytes: 2, offset);

        if (bytes is null)
        {
            return null;
        }

        return BitConverter.ToUInt16(bytes, startIndex: 0);
    }

    /// <summary>
    ///     Creates a new Span of T object over the entirety of a specified array.
    /// </summary>
    public static Span<T> ToSpan<T>(this T[]? values) => values is null ? [] : values.AsSpan();

    /// <summary>
    ///     Creates a new Span of T object over the entirety of a specified array.
    /// </summary>
    public static Span<T> ToSpan<T>(this T[]? values, int start, int length)
        => values is null ? [] : values.AsSpan(start, length);

    /// <summary>
    ///     Gets a Span of T view over the data in a list. Items should not be added or removed from the List of T while the
    ///     Span of T is in use.
    /// </summary>
    public static Span<T> ToSpan<T>(this List<T>? values) => values is null ? [] : CollectionsMarshal.AsSpan(values);
}
