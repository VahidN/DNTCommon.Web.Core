using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
///     SerializationProvider Extensions
/// </summary>
public static class SerializationProviderExtensions
{
    /// <summary>
    ///     Adds ISerializationProvider to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddSerializationProvider(this IServiceCollection services)
    {
        services.AddSingleton<ISerializationProvider, SerializationProvider>();

        return services;
    }

    /// <summary>
    ///     Provides options to be used with JsonSerializer to be used with ASP.NET Core apps.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddCustomJsonOptionsForWebApps(this IMvcBuilder builder)
    {
        var serializerOptions = DefaultJsonSerializerOptions.Instance;

        return builder.AddJsonOptions(options =>
        {
            var jsonSerializerOptions = options.JsonSerializerOptions;
            jsonSerializerOptions.NumberHandling = serializerOptions.NumberHandling;
            jsonSerializerOptions.PropertyNameCaseInsensitive = serializerOptions.PropertyNameCaseInsensitive;
            jsonSerializerOptions.WriteIndented = serializerOptions.WriteIndented;
            jsonSerializerOptions.DefaultIgnoreCondition = serializerOptions.DefaultIgnoreCondition;
            jsonSerializerOptions.Converters.AddRange(serializerOptions.Converters);
            jsonSerializerOptions.Encoder = serializerOptions.Encoder;
            jsonSerializerOptions.ReferenceHandler = serializerOptions.ReferenceHandler;
        });
    }

    /// <summary>
    ///     Serialize the given data to a string.
    /// </summary>
    public static string Serialize(this object data, JsonSerializerOptions options)
        => JsonSerializer.Serialize(data, options);

    /// <summary>
    ///     Serialize the given data to a string.
    /// </summary>
    public static string Serialize(this object data)
        => JsonSerializer.Serialize(data, DefaultJsonSerializerOptions.Instance);

    /// <summary>
    ///     Deserialize the given string to an object.
    /// </summary>
    public static T? Deserialize<T>(this string data)
        => JsonSerializer.Deserialize<T>(data, DefaultJsonSerializerOptions.Instance);

    /// <summary>
    ///     Deserialize the given string to an object.
    /// </summary>
    public static T? Deserialize<T>(this string data, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<T>(data, options);

    /// <summary>
    ///     Deserialize the given stream to an object.
    /// </summary>
    public static ValueTask<T?> DeserializeAsync<T>(this Stream utf8Json,
        JsonSerializerOptions options,
        CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync<T>(utf8Json, options, cancellationToken);

    /// <summary>
    ///     Deserialize the given stream to an object.
    /// </summary>
    public static ValueTask<T?> DeserializeAsync<T>(this Stream utf8Json, CancellationToken cancellationToken = default)
        => JsonSerializer.DeserializeAsync<T>(utf8Json, DefaultJsonSerializerOptions.Instance, cancellationToken);

    /// <summary>
    ///     Serialize the given data to a byte array.
    /// </summary>
    public static byte[] SerializeToUtf8Bytes(this object data, JsonSerializerOptions options)
        => JsonSerializer.SerializeToUtf8Bytes(data, options);

    /// <summary>
    ///     Serialize the given data to a byte array.
    /// </summary>
    public static byte[] SerializeToUtf8Bytes(this object data)
        => JsonSerializer.SerializeToUtf8Bytes(data, DefaultJsonSerializerOptions.Instance);

    /// <summary>
    ///     Deserialize the given byte array to an object.
    /// </summary>
    public static T? DeserializeFromUtf8Bytes<T>(this byte[] data)
        => JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(data), DefaultJsonSerializerOptions.Instance);

    /// <summary>
    ///     Deserialize the given byte array to an object.
    /// </summary>
    public static T? DeserializeFromUtf8Bytes<T>(this byte[] data, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(data), options);

    /// <summary>
    ///     Asynchronously reads the UTF-8 encoded text representing a single JSON value into an instance of a type specified
    ///     by a generic type parameter. The stream will be read to completion.
    /// </summary>
    public static async ValueTask<T?> DeserializeJsonFileAsync<T>(this string fileNamePath,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(fileNamePath))
        {
            throw new FileNotFoundException(fileNamePath);
        }

        options ??= DefaultJsonSerializerOptions.Instance;

        await using var stream = fileNamePath.ToFileStream(FileMode.Open, FileAccess.Read, useAsync: true);

        return stream is null ? default : await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
    }
}
