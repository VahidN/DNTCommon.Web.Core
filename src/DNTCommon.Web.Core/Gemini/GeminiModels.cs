namespace DNTCommon.Web.Core;

public sealed class GeminiModels
{
    public static readonly GeminiModels Gemini25Flash = "gemini-2.5-flash";
    public static readonly GeminiModels Gemini25FlashLite = "gemini-2.5-flash-lite";
    public static readonly GeminiModels Gemini25FlashTts = "gemini-2.5-flash-tts";	

    private GeminiModels(string value) => Value = value;

    public string Value { get; }

    public static implicit operator GeminiModels(string value) => new(value);

    public override string ToString() => Value;

    public GeminiModels ToGeminiModels(string value) => new(value);
}
