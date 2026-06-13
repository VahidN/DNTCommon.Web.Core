using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNTCommon.Web.Core;

public static class BaleBot
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task<BaleApiResponseStatus?> SendFileToBaleChannelAsync(this HttpClient baleClient,
        string baleBotToken,
        string channelId,
        BaleFileType fileType,
        string filePath,
        string? caption,
        BaleParseMode parseMode,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(baleClient);
        ArgumentNullException.ThrowIfNull(baleBotToken);
        ArgumentNullException.ThrowIfNull(channelId);

        if (!filePath.FileExists())
        {
            return null;
        }

        using var multipartContent = new MultipartFormDataContent();

        await using var fileStream = File.OpenRead(filePath);
        using var fileContent = new StreamContent(fileStream);

        var fileName = filePath.GetFileName()!;

        multipartContent.Add(fileContent, fileType.ToString().ToLowerInvariant(), fileName);
        multipartContent.Add(new StringContent(channelId), name: "chat_id");
        multipartContent.Add(new StringContent(caption ?? fileName), name: "caption");

        if (parseMode != BaleParseMode.None)
        {
            multipartContent.Add(new StringContent(parseMode switch
            {
                BaleParseMode.Markdown => "Markdown",
                BaleParseMode.MarkdownV2 => "MarkdownV2",
                BaleParseMode.Html => "HTML",
                _ => throw new ArgumentOutOfRangeException(nameof(parseMode))
            }), name: "parse_mode");
        }

        var endpoint = fileType switch
        {
            BaleFileType.Photo => "sendPhoto",
            BaleFileType.Video => "sendVideo",
            BaleFileType.Audio => "sendAudio",
            BaleFileType.Document => "sendDocument",
            _ => throw new ArgumentException($"Unsupported file type: {fileType}", nameof(fileType))
        };

        using var response = await baleClient.PostAsync($"https://tapi.bale.ai/bot{baleBotToken}/{endpoint}",
            multipartContent, cancellationToken);

        return await response.IsBaleApiResponseOkAsync(cancellationToken);
    }

    public static async Task<BaleApiResponseStatus> SendTextMessageToBaleChannelAsync(this HttpClient baleClient,
        string baleBotToken,
        string channelId,
        string message,
        BaleParseMode parseMode,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(baleClient);
        ArgumentNullException.ThrowIfNull(baleBotToken);
        ArgumentNullException.ThrowIfNull(channelId);

        var request = new Dictionary<string, object?>
        {
            [key: "chat_id"] = channelId,
            [key: "text"] = message
        };

        if (parseMode != BaleParseMode.None)
        {
            request[key: "parse_mode"] = parseMode switch
            {
                BaleParseMode.Markdown => "Markdown",
                BaleParseMode.MarkdownV2 => "MarkdownV2",
                BaleParseMode.Html => "HTML",
                _ => null
            };
        }

        using var content = new StringContent(JsonSerializer.Serialize(request, JsonSerializerOptions), Encoding.UTF8,
            mediaType: "application/json");

        using var response = await baleClient.PostAsync($"https://tapi.bale.ai/bot{baleBotToken}/sendMessage", content,
            cancellationToken);

        return await response.IsBaleApiResponseOkAsync(cancellationToken);
    }

    public static async Task<BaleApiResponseStatus> IsBaleApiResponseOkAsync(this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new BaleApiResponseStatus(Success: false, response.StatusCode, responseContent);
        }

        var apiResponse = JsonSerializer.Deserialize<BaleApiResponse>(responseContent);

        return new BaleApiResponseStatus(apiResponse?.Ok == true, response.StatusCode, responseContent);
    }
}
