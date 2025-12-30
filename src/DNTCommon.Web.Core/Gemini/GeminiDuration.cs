using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     A Duration represents a signed, fixed-length span of time represented as a count of seconds and fractions
///     of seconds at nanosecond resolution.
/// </summary>
/// <seealso href="https://developers.google.com/protocol-buffers/docs/reference/google.protobuf#google.protobuf.Duration">
///     See
///     Official Protobuf Documentation
/// </seealso>
[JsonConverter(typeof(GeminiDurationJsonConverter))]
public class GeminiDuration
{
    /// <summary>
    ///     Signed seconds of the span of time. Must be from -315,576,000,000 to +315,576,000,000 inclusive.
    /// </summary>
    public long Seconds { get; set; }

    /// <summary>
    ///     Signed fractions of a second at nanosecond resolution of the span of time. Durations less than one second are
    ///     represented with a 0 <see cref="Seconds" /> field and a positive or negative <see cref="Nanos" /> field. For
    ///     durations
    ///     of one second or more, a non-zero value for the <see cref="Nanos" /> field must be of the same sign as the
    ///     <see cref="Seconds" />
    ///     field. Must be from -999,999,999 to +999,999,999 inclusive.
    /// </summary>
    public int Nanos { get; set; }

    /// <summary>
    ///     Implicitly converts a <see cref="TimeSpan" /> object to a <see cref="TimeSpan" /> object.
    /// </summary>
    /// <param name="timeSpan">The <see cref="GeminiDuration" /> object to convert.</param>
    public static implicit operator GeminiDuration(TimeSpan timeSpan) => FromTimeSpan(timeSpan);

    /// <summary>
    ///     Implicitly converts a <see cref="TimeSpan" /> object to a <see cref="GeminiDuration" /> object.
    /// </summary>
    /// <param name="duration">The <see cref="GeminiDuration" /> object to convert.</param>
    public static implicit operator TimeSpan(GeminiDuration duration)
    {
        ArgumentNullException.ThrowIfNull(duration);

        return duration.ToTimeSpan();
    }

    /// <summary>
    ///     Converts this <see cref="TimeSpan" /> object to a <see cref="TimeSpan" /> object.
    /// </summary>
    /// <returns>The converted <see cref="GeminiDuration" /> object.</returns>
    public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(Seconds) + TimeSpan.FromTicks(Nanos / 100);

    /// <summary>
    ///     Creates a new <see cref="TimeSpan" /> object from a <see cref="TimeSpan" /> object.
    /// </summary>
    /// <param name="timeSpan">The <see cref="GeminiDuration" /> object to convert.</param>
    /// <returns>The new <see cref="GeminiDuration" /> object.</returns>
    public static GeminiDuration FromTimeSpan(TimeSpan timeSpan)
        => new()
        {
            Seconds = (long)timeSpan.TotalSeconds,
            Nanos = (int)(timeSpan.Ticks % TimeSpan.TicksPerSecond) * 100
        };
}
