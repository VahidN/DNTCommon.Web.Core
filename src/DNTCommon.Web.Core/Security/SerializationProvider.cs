using Microsoft.Extensions.DependencyInjection;
#if NETCOREAPP3_0 || NETCOREAPP3_1
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif

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

#if NETCOREAPP3_0 || NETCOREAPP3_1
        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        string Serialize(object data, JsonSerializerOptions options);

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        T Deserialize<T>(string data, JsonSerializerOptions options);
#else
        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        string Serialize(object data, JsonSerializerSettings options);

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        T Deserialize<T>(string data, JsonSerializerSettings options);
#endif
    }

    /// <summary>
    /// Serialization Provider
    /// </summary>
    public class SerializationProvider : ISerializationProvider
    {
#if NETCOREAPP3_0 || NETCOREAPP3_1
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
#else
        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        public string Serialize(object data, JsonSerializerSettings options)
        {
            return JsonConvert.SerializeObject(data, options);
        }

        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data,
                                                new JsonSerializerSettings
                                                {
                                                    Formatting = Formatting.None,
                                                    NullValueHandling = NullValueHandling.Ignore,
                                                    DefaultValueHandling = DefaultValueHandling.Include
                                                });
        }
#endif

#if NETCOREAPP3_0 || NETCOREAPP3_1
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
#else
        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        public T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);

        }

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        public T Deserialize<T>(string data, JsonSerializerSettings options)
        {
            return JsonConvert.DeserializeObject<T>(data, options);

        }
#endif
    }
}