namespace DNTCommon.Web.Core;

public sealed class GeminiClientOptions
{
    public string? ApiKey { get; set; }

    public GeminiApiVersions? ApiVersion { get; set; }

    public GeminiModels? ModelId { get; set; }

    public string? SystemInstruction { get; set; }

    public ICollection<GeminiChatRequest>? Chats { get; set; }

    public ICollection<GeminiResponseModality>? ResponseModalities { set; get; }

    public int? MaxOutputTokens { get; set; }
}