namespace DNTCommon.Web.Core;

public class GeminiChatRequest
{
    public GeminiChatRequest()
    {
    }

    public GeminiChatRequest(string content) => Content = content;

    public GeminiChatRequest(GeminiBlob inlineData) => InlineData = inlineData;

    public GeminiChatRequest(GeminiFileData fileData) => FileData = fileData;

    public string? Content { set; get; }

    /// <summary>
    ///     Raw data with a specified media type.
    /// </summary>
    public GeminiBlob? InlineData { set; get; }

    /// <summary>
    ///     URI-based data with a specified media type.
    /// </summary>
    public GeminiFileData? FileData { get; set; }
}