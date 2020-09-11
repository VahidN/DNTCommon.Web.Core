using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// SerializationProvider Extensions
    /// </summary>
    public static class SerializationProviderExtensions
    {
        /// <summary>
        /// Adds ISerializationProvider to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddSerializationProvider(this IServiceCollection services)
        {
            services.AddSingleton<ISerializationProvider, SerializationProvider>();
            return services;
        }
    }

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
        /// Deserialize the given string to an object.
        /// </summary>
        T Deserialize<T>(string data);

        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        string Serialize(object data, JsonSerializerOptions options);

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        T Deserialize<T>(string data, JsonSerializerOptions options);
    }

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
                                                 IgnoreNullValues = true
                                             });
        }

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        public T Deserialize<T>(string data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        public T Deserialize<T>(string data, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<T>(data, options);
        }
    }
}