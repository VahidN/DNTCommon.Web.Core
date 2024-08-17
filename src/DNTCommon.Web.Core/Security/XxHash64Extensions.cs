using System.IO.Hashing;
using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     Provides an implementation of the XxHash64 algorithm.
/// </summary>
public static class XxHash64Extensions
{
    /// <summary>
    ///     Computes the XxHash64 hash of the provided data.
    /// </summary>
    public static ulong ToXxHash64(this string data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return XxHash64.HashToUInt64(Encoding.UTF8.GetBytes(data));
    }

    /// <summary>
    ///     Computes the XxHash64 hash of the provided data.
    /// </summary>
    public static ulong ToXxHash64(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return XxHash64.HashToUInt64(data);
    }

    /// <summary>
    ///     Computes the XxHash64 hash of the provided data.
    /// </summary>
    public static ulong ToXxHash64(this byte[] data, int offset, int len, uint seed)
    {
        ArgumentNullException.ThrowIfNull(data);

        return XxHash64.HashToUInt64(data.AsSpan(offset, len), seed);
    }
}