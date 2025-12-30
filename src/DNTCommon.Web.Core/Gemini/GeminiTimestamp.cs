using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     A Timestamp represents a point in time independent of any time zone or calendar,
///     represented as seconds and fractions of seconds at nanosecond resolution in UTC Epoch time.
/// </summary>
/// <seealso href="https://developers.google.com/protocol-buffers/docs/reference/google.protobuf#google.protobuf.Timestamp">
///     See
///     Official Protobuf Documentation
/// </seealso>
[JsonConverter(typeof(GeminiTimestampJsonConverter))]
public class GeminiTimestamp
{
    /// <summary>
    ///     Represents seconds of UTC time since Unix epoch 1970-01-01T00:00:00Z. Must be from 0001-01-01T00:00:00Z to
    ///     9999-12-31T23:59:59Z inclusive.
    /// </summary>
    public long Seconds { get; set; }

    /// <summary>
    ///     Non-negative fractions of a second at nanosecond resolution. Negative second values with fractions
    ///     must still have non-negative nanos values that count forward in time. Must be from 0 to 999,999,999
    ///     inclusive.
    /// </summary>
    public int Nanos { get; set; }

    /// <summary>
    ///     Implicitly converts a <see cref="DateTime" /> object to a <see cref="DateTime" /> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="GeminiTimestamp" /> object to convert.</param>
    public static implicit operator GeminiTimestamp(DateTime dateTime) => FromDateTime(dateTime);

    /// <summary>
    ///     Implicitly converts a <see cref="DateTime" /> object to a <see cref="GeminiTimestamp" /> object.
    /// </summary>
    /// <param name="timestamp">The <see cref="GeminiTimestamp" /> object to convert.</param>
    public static implicit operator DateTime(GeminiTimestamp timestamp)
    {
        ArgumentNullException.ThrowIfNull(timestamp);

        return timestamp.ToDateTime();
    }

    /// <summary>
    ///     Converts this <see cref="DateTime" /> object to a <see cref="DateTime" /> object.
    /// </summary>
    /// <returns>The converted <see cref="GeminiTimestamp" /> object.</returns>
    public DateTime ToDateTime() => DateTimeOffset.FromUnixTimeSeconds(Seconds).AddTicks(Nanos / 100).UtcDateTime;

    /// <summary>
    ///     Creates a new <see cref="DateTime" /> object from a <see cref="DateTime" /> object.
    /// </summary>
    /// <param name="dateTime">The <see cref="GeminiTimestamp" /> object to convert.</param>
    /// <returns>The new <see cref="GeminiTimestamp" /> object.</returns>
    public static GeminiTimestamp FromDateTime(DateTime dateTime)
    {
        var dateTimeOffset = new DateTimeOffset(dateTime);

        return new GeminiTimestamp
        {
            Seconds = dateTimeOffset.ToUnixTimeSeconds(),
            Nanos = (int)(dateTimeOffset.Ticks % TimeSpan.TicksPerSecond) * 100
        };
    }
}
