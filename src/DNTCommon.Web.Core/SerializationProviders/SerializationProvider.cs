using System.Text.Json;

namespace DNTCommon.Web.Core;

/// <summary>
///     Serialization Provider
/// </summary>
public class SerializationProvider : ISerializationProvider
{
    /// <summary>
    ///     Serialize the given data to a string.
    /// </summary>
    public string Serialize(object data, JsonSerializerOptions options) => data.Serialize(options);

    /// <summary>
    ///     Serialize the given data to a string.
    /// </summary>
    public string Serialize(object data) => data.Serialize();

    /// <summary>
    ///     Deserialize the given string to an object.
    /// </summary>
    public T? Deserialize<T>(string data) => data.Deserialize<T>();

    /// <summary>
    ///     Deserialize the given string to an object.
    /// </summary>
    public T? Deserialize<T>(string data, JsonSerializerOptions options) => data.Deserialize<T>(options);

    /// <summary>
    ///     Serialize the given data to a byte array.
    /// </summary>
    public byte[] SerializeToUtf8Bytes(object data, JsonSerializerOptions options)
        => data.SerializeToUtf8Bytes(options);

    /// <summary>
    ///     Serialize the given data to a byte array.
    /// </summary>
    public byte[] SerializeToUtf8Bytes(object data) => data.SerializeToUtf8Bytes();

    /// <summary>
    ///     Deserialize the given byte array to an object.
    /// </summary>
    public T? DeserializeFromUtf8Bytes<T>(byte[] data) => data.DeserializeFromUtf8Bytes<T>();

    /// <summary>
    ///     Deserialize the given byte array to an object.
    /// </summary>
    public T? DeserializeFromUtf8Bytes<T>(byte[] data, JsonSerializerOptions options)
        => data.DeserializeFromUtf8Bytes<T>(options);

    /// <summary>
    ///     Deserialize the given stream to an object.
    /// </summary>
    public ValueTask<T?> DeserializeAsync<T>(Stream utf8Json,
        JsonSerializerOptions options,
        CancellationToken cancellationToken = default)
        => utf8Json.DeserializeAsync<T>(options, cancellationToken);

    /// <summary>
    ///     Deserialize the given stream to an object.
    /// </summary>
    public ValueTask<T?> DeserializeAsync<T>(Stream utf8Json, CancellationToken cancellationToken = default)
        => utf8Json.DeserializeAsync<T>(cancellationToken);
}