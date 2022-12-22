using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     Serialization Provider
/// </summary>
public class SerializationProvider : ISerializationProvider
{
    /// <summary>
    ///     Serialize the given data to an string.
    /// </summary>
    public string Serialize(object data, JsonSerializerOptions options) => JsonSerializer.Serialize(data, options);

    /// <summary>
    ///     Serialize the given data to an string.
    /// </summary>
    public string Serialize(object data) =>
        JsonSerializer.Serialize(data,
                                 new JsonSerializerOptions
                                 {
                                     WriteIndented = false,
                                     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                 });

    /// <summary>
    ///     Deserialize the given string to an object.
    /// </summary>
    public T? Deserialize<T>(string data) => JsonSerializer.Deserialize<T>(data);

    /// <summary>
    ///     Deserialize the given string to an object.
    /// </summary>
    public T? Deserialize<T>(string data, JsonSerializerOptions options) =>
        JsonSerializer.Deserialize<T>(data, options);

    /// <summary>
    ///     Serialize the given data to a byte array.
    /// </summary>
    public byte[] SerializeToUtf8Bytes(object data, JsonSerializerOptions options) =>
        JsonSerializer.SerializeToUtf8Bytes(data, options);

    /// <summary>
    ///     Serialize the given data to a byte array.
    /// </summary>
    public byte[] SerializeToUtf8Bytes(object data) =>
        JsonSerializer.SerializeToUtf8Bytes(data,
                                            new JsonSerializerOptions
                                            {
                                                WriteIndented = false,
                                                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                            });

    /// <summary>
    ///     Deserialize the given byte array to an object.
    /// </summary>
    public T? DeserializeFromUtf8Bytes<T>(byte[] data) => JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(data));

    /// <summary>
    ///     Deserialize the given byte array to an object.
    /// </summary>
    public T? DeserializeFromUtf8Bytes<T>(byte[] data, JsonSerializerOptions options) =>
        JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(data), options);
}