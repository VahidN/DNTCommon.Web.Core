using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public sealed class JsonStringBase64Converter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.String
            ? reader.GetBytesFromBase64()
            : throw new JsonException(message: "Expected a Base64 string");

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteBase64StringValue(value);
    }
}