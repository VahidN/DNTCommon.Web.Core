using System.Text.Json;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Serialization Provider
    /// </summary>
    public interface ISerializationProvider
    {
        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        string Serialize(object data);

        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        string Serialize(object data, JsonSerializerOptions options);

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        T? Deserialize<T>(string data, JsonSerializerOptions options);

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        T? Deserialize<T>(string data);

        /// <summary>
        /// Serialize the given data to a byte array.
        /// </summary>
        byte[] SerializeToUtf8Bytes(object data, JsonSerializerOptions options);

        /// <summary>
        /// Serialize the given data to a byte array.
        /// </summary>
        byte[] SerializeToUtf8Bytes(object data);

        /// <summary>
        /// Deserialize the given byte array to an object.
        /// </summary>
        T? DeserializeFromUtf8Bytes<T>(byte[] data);

        /// <summary>
        /// Deserialize the given byte array to an object.
        /// </summary>
        T? DeserializeFromUtf8Bytes<T>(byte[] data, JsonSerializerOptions options);
    }
}