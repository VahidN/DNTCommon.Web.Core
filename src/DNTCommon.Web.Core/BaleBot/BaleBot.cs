using System.Text;
using System.Text.Json;

namespace DNTCommon.Web.Core;

public static class BaleBot
{
    public static async Task<BaleApiResponseStatus?> SendFileToBaleChannelAsync(this HttpClient baleClient,
        string baleBotToken,
        string channelId,
        BaleFileType fileType,
        string filePath,
        string? caption,
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

        var readAllBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
        using var fileContent = new ByteArrayContent(readAllBytes);
        var fileName = filePath.GetFileName()!;
        multipartContent.Add(fileContent, fileType.ToString().ToLowerInvariant(), fileName);

        using var channelIdStr = new StringContent(channelId);
        multipartContent.Add(channelIdStr, name: "chat_id");

        using var captionStr = new StringContent(caption ?? fileName);
        multipartContent.Add(captionStr, name: "caption");

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
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(baleClient);
        ArgumentNullException.ThrowIfNull(baleBotToken);
        ArgumentNullException.ThrowIfNull(channelId);

        using var content = new StringContent(JsonSerializer.Serialize(new
        {
            chat_id = channelId,
            text = message
        }), Encoding.UTF8, mediaType: "application/json");

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
