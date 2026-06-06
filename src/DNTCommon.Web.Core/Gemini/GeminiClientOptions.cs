namespace DNTCommon.Web.Core;

public sealed class GeminiClientOptions
{
    public string? ApiKey { get; set; }

    public GeminiApiVersions? ApiVersion { get; set; }

    /// <summary>
    ///     Such as gemma-3-12b-it or gemini-2.5-flash, etc
    /// </summary>
    public string? ModelId { get; set; }

    public string? SystemInstruction { get; set; }

    public ICollection<GeminiChatRequest>? Chats { get; set; }

    /// <summary>
    ///     Supported modalities of the response.
    /// </summary>
    public ICollection<GeminiResponseModality>? ResponseModalities { get; set; }

    /// <summary>
    ///     The maximum number of tokens to include in a response candidate.
    /// </summary>
    public int? MaxOutputTokens { get; set; }

    /// <summary>
    ///     Optional. MIME type of the generated candidate text.
    ///     Supported MIME types are:
    ///     text/plain: (default) Text output.
    ///     application/json: JSON response in the response candidates.
    ///     text/x.enum: ENUM as a string response in the response candidates.
    ///     Refer to the docs https://ai.google.dev/gemini-api/docs/prompting_with_media#plain_text_formats
    ///     for a list of all supported text MIME types.
    /// </summary>
    public string? ResponseMimeType { get; set; }

    /// <summary>
    ///     Optional. Config for thinking features. An error will be returned if this field is set for models that don't
    ///     support thinking.
    /// </summary>
    public bool DisableThinking { get; set; }
}
