namespace DNTCommon.Web.Core;

public sealed class GeminiModels
{
    // Gemini 2.5 (Preview)
    public static readonly GeminiModels GeminiProVision = "gemini-pro-vision";
    public static readonly GeminiModels Gemini25ProPreview = "gemini-2.5-pro-preview";
    public static readonly GeminiModels Gemini25FlashPreview = "gemini-2.5-flash-preview";
    public static readonly GeminiModels Gemini25ProExp0325 = "gemini-2.5-pro-exp-03-25";

    // Gemini 2.0
    public static readonly GeminiModels Gemini2Flash = "gemini-2.0-flash";
    public static readonly GeminiModels Gemini2FlashExp = "gemini-2.0-flash-exp";

    public static readonly GeminiModels Gemini2FlashPreviewImageGeneration =
        "gemini-2.0-flash-preview-image-generation";

    public static readonly GeminiModels Gemini2FlashLite = "gemini-2.0-flash-lite";
    public static readonly GeminiModels Gemini2FlashLive = "gemini-2.0-flash-live";

    public static readonly GeminiModels Gemini2FlashThinkingExperimental = "gemini-2.0-flash-thinking-experimental";

    public static readonly GeminiModels Gemini2ProExperimental = "gemini-2.0-pro-experimental";

    // Gemini 1.5
    public static readonly GeminiModels Gemini15Pro = "gemini-1.5-pro";
    public static readonly GeminiModels Gemini15Flash = "gemini-1.5-flash";
    public static readonly GeminiModels Gemini15Flash8B = "gemini-1.5-flash-8b";

    // Other Models
    public static readonly GeminiModels Imagen3 = "imagen-3";
    public static readonly GeminiModels Veo2 = "veo-2";
    public static readonly GeminiModels GeminiEmbeddingExperimental = "gemini-embedding-experimental";
    public static readonly GeminiModels TextEmbedding = "text-embedding";
    public static readonly GeminiModels AQA = "aqa";
    public static readonly GeminiModels TextEmbedding004 = "text-embedding-004";

    private GeminiModels(string value) => Value = value;

    public string Value { get; }

    public static implicit operator GeminiModels(string value) => new(value);

    public override string ToString() => Value;

    public GeminiModels ToGeminiModels(string value) => new(value);
}
