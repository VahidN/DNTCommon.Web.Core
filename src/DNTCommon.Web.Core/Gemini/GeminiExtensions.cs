using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public static class GeminiExtensions
{
    public static readonly JsonSerializerOptions DeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true,

        // In case the API returns numbers as strings
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public static readonly JsonSerializerOptions SerializeOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static GeminiErrorResponse? GetGeminiErrorResponse(this HttpResponseMessage response, string responseBody)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.IsSuccessStatusCode)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<GeminiErrorResponse>(responseBody, DeserializeOptions);
        }
        catch (Exception)
        {
            return new GeminiErrorResponse
            {
                Error = new GeminiErrorDetails
                {
                    StatusCode = response.StatusCode,
                    Message = responseBody
                }
            };
        }
    }

    public static string? DecodeBase64(this string base64EncodedString)
    {
        if (string.IsNullOrEmpty(base64EncodedString))
        {
            return string.Empty;
        }

        try
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedString);

            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public static bool SupportsTextOutput(this GeminiModelInfo? modelInfo)
    {
        if (modelInfo is null)
        {
            return false;
        }

        var modelName = modelInfo.Name;

        // 1. Exclude specialized media-only models by name suffix
        if (modelName.Contains(value: "tts", StringComparison.OrdinalIgnoreCase) ||
            modelName.Contains(value: "image", StringComparison.OrdinalIgnoreCase) ||
            modelName.Contains(value: "audio", StringComparison.OrdinalIgnoreCase) ||
            modelName.Contains(value: "robot", StringComparison.OrdinalIgnoreCase) ||
            modelName.Contains(value: "veo", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // 2. Ensure it supports standard content generation
        var hasGenerateMethod =
            modelInfo.SupportedGenerationMethods.Contains(value: "generateContent", StringComparer.OrdinalIgnoreCase);

        // 3. Exclude Embedding models (which output vectors, not text)
        var isEmbedding = modelName.Contains(value: "embedding", StringComparison.OrdinalIgnoreCase);

        return hasGenerateMethod && !isEmbedding;
    }

    public static string GetRemoteGeminiFileId(this string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);

        if (!fileName.Contains(value: '/', StringComparison.Ordinal))
        {
            return $"files/{fileName}";
        }

        if (fileName.StartsWith(value: "files/", StringComparison.OrdinalIgnoreCase))
        {
            return fileName;
        }

        throw new ArgumentException($"Invalid file name. {fileName}");
    }

    public static Uri GetFullApiUri(this GeminiClientOptions options, string baseUrl)
    {
        ArgumentNullException.ThrowIfNull(options);

        var modelId = options.ModelId.TrimStart(value: "models/", StringComparison.OrdinalIgnoreCase);

        return new Uri($"{baseUrl}/{options.ApiVersion}/models/{modelId}:generateContent?key={options.ApiKey}");
    }

    public static GeminiGenerateContentRequest CreateGeminiContentRequest(this GeminiClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new GeminiGenerateContentRequest
        {
            SystemInstruction = string.IsNullOrWhiteSpace(options.SystemInstruction)
                ? null
                : new GeminiContent
                {
                    Parts =
                    [
                        new GeminiPart
                        {
                            Text = options.SystemInstruction
                        }
                    ]
                },
            Contents =
            [
                new GeminiContent
                {
                    Role = GeminiChatRoles.User,
                    Parts = GetParts(options)
                }
            ],
            GenerationConfig = new GeminiGenerationConfig
            {
                Temperature = 0.3f,
                TopK = 1,
                TopP = 1,
                MaxOutputTokens = options.MaxOutputTokens,
                StopSequences = [],
                ResponseModalities = options.ResponseModalities,
                ResponseMimeType = options.ResponseMimeType
            },
            SafetySettings =
            [
                new GeminiSafetySettings
                {
                    Category = "HARM_CATEGORY_HARASSMENT",
                    Threshold = "BLOCK_NONE"
                },
                new GeminiSafetySettings
                {
                    Category = "HARM_CATEGORY_HATE_SPEECH",
                    Threshold = "BLOCK_NONE"
                },
                new GeminiSafetySettings
                {
                    Category = "HARM_CATEGORY_SEXUALLY_EXPLICIT",
                    Threshold = "BLOCK_NONE"
                },
                new GeminiSafetySettings
                {
                    Category = "HARM_CATEGORY_DANGEROUS_CONTENT",
                    Threshold = "BLOCK_NONE"
                }
            ]
        };
    }

    private static List<GeminiPart>? GetParts(GeminiClientOptions? options)
    {
        if (options?.Chats is null)
        {
            return null;
        }

        var parts = new List<GeminiPart>();

        foreach (var chatRequest in options.Chats)
        {
            if (!string.IsNullOrWhiteSpace(chatRequest.Content))
            {
                parts.Add(new GeminiPart
                {
                    Text = chatRequest.Content
                });
            }

            if (chatRequest.InlineData is not null)
            {
                parts.Add(new GeminiPart
                {
                    InlineData = chatRequest.InlineData
                });
            }

            if (chatRequest.FileData is not null)
            {
                parts.Add(new GeminiPart
                {
                    FileData = chatRequest.FileData
                });
            }
        }

        return parts;
    }
}
