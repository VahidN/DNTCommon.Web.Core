using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Serialization Provider
    /// </summary>
    public class SerializationProvider : ISerializationProvider
    {
        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        public string Serialize(object data, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(data, options);
        }

        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        public string Serialize(object data)
        {
            return JsonSerializer.Serialize(data,
            new JsonSerializerOptions
            {
                WriteIndented = false,
#if !NETCORE3_1
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
#else
                IgnoreNullValues = true
#endif
            });
        }

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        public T? Deserialize<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        public T? Deserialize<T>(string data, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(data, options);
        }

        /// <summary>
        /// Serialize the given data to a byte array.
        /// </summary>
        public byte[] SerializeToUtf8Bytes(object data, JsonSerializerOptions options)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data, options);
        }

        /// <summary>
        /// Serialize the given data to a byte array.
        /// </summary>
        public byte[] SerializeToUtf8Bytes(object data)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data,
            new JsonSerializerOptions
            {
                WriteIndented = false,
#if !NETCORE3_1
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
#else
                IgnoreNullValues = true
#endif
            });
        }

        /// <summary>
        /// Deserialize the given byte array to an object.
        /// </summary>
        public T? DeserializeFromUtf8Bytes<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(data));
        }

        /// <summary>
        /// Deserialize the given byte array to an object.
        /// </summary>
        public T? DeserializeFromUtf8Bytes<T>(byte[] data, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(data), options);
        }
    }
}