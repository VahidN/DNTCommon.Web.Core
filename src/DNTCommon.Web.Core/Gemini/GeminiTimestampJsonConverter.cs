using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     JSON converter for <see cref="GeminiTimestamp" />.
/// </summary>
public class GeminiTimestampJsonConverter : JsonConverter<GeminiTimestamp>
{
    /// <inheritdoc />
    public override GeminiTimestamp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Assuming the JSON representation is a string in RFC 3339 format
        var timestampString = reader.GetString() ??
                              throw new InvalidOperationException(message: "timestampString is null.");

        return GeminiTimestamp.FromDateTime(DateTime.Parse(timestampString, CultureInfo.InvariantCulture));
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, GeminiTimestamp value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);

        // Assuming the JSON representation is a string in RFC 3339 format
        writer.WriteStringValue(value.ToDateTime().ToString(format: "o"));

        // "o" format specifier for RFC 3339
    }
}
