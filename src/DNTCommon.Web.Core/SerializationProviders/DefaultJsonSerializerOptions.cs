using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace DNTCommon.Web.Core;

/// <summary>
///     Provides options to be used with JsonSerializer.
/// </summary>
public static class DefaultJsonSerializerOptions
{
    /// <summary>
    ///     Provides options to be used with JsonSerializer.
    /// </summary>
    public static readonly JsonSerializerOptions Instance = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter()
        },
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
}