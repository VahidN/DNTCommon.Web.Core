using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     JSON converter for <see cref="GeminiDuration" />.
/// </summary>
public class GeminiDurationJsonConverter : JsonConverter<GeminiDuration>
{
    /// <inheritdoc />
    public override GeminiDuration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Parse the duration string (e.g., "3.5s")
        var durationString = reader.GetString();
        var duration = double.Parse(durationString!.TrimEnd(trimChar: 's'), CultureInfo.InvariantCulture);

        // Convert the duration to seconds and nanoseconds
        var seconds = (long)duration;
        var nanos = (int)((duration - seconds) * 1_000_000_000);

        return new GeminiDuration
        {
            Seconds = seconds,
            Nanos = nanos
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, GeminiDuration value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);

        // Convert seconds and nanoseconds to a duration string with the specified format
        var duration = value.Seconds + ((double)value.Nanos / 1_000_000_000);
        writer.WriteStringValue(string.Create(CultureInfo.InvariantCulture, $"{duration:F9}s"));
    }
}
