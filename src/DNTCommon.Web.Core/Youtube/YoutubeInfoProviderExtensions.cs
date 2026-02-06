using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.WebUtilities;

namespace DNTCommon.Web.Core;

public static partial class YoutubeInfoProviderExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<byte[]?> GetYoutubeVideoThumbnailDataAsync(this HttpClient httpClient,
        string? videoId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        if (videoId.IsEmpty())
        {
            return null;
        }

        var thumbnailUrl = $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg";

        return await httpClient.DownloadDataAsync(thumbnailUrl, cancellationToken: cancellationToken);
    }

    public static (bool Success, string? VideoId) IsYoutubeVideo(this string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return (false, null);
        }

        var uri = new Uri(url);

        if (uri.Host.Contains(value: ".youtube.", StringComparison.OrdinalIgnoreCase) && uri.Segments.Length >= 3 &&
            (uri.Segments[1].Equals(value: "embed/", StringComparison.OrdinalIgnoreCase) ||
             uri.Segments[1].Equals(value: "shorts/", StringComparison.OrdinalIgnoreCase)))
        {
            return (true, uri.Segments[2]);
        }

        if (uri.Host.Contains(value: ".youtube.", StringComparison.OrdinalIgnoreCase))
        {
            var queryDictionary = QueryHelpers.ParseQuery(uri.Query);

            return !queryDictionary.TryGetValue(key: "v", out var v) ? (false, null) : (true, v.ToString());
        }

        if (uri.Host.EndsWith(value: "youtu.be", StringComparison.OrdinalIgnoreCase) && uri.Segments.Length >= 2)
        {
            return (true, uri.Segments[1]);
        }

        return (false, null);
    }

    public static async Task<string?> GetYoutubeVideoDescriptionAsync(this HttpClient httpClient,
        string? url,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        if (url.IsEmpty())
        {
            return null;
        }

        var htmlContentResult = await httpClient.SafeFetchAsync(url, cancellationToken: cancellationToken);

        if (htmlContentResult.Kind != FetchResultKind.Success || htmlContentResult.TextContent.IsEmpty())
        {
            return null;
        }

#if !NET_6
        var match = PlayerResponse().Match(htmlContentResult.TextContent);
#else
        var match = PlayerResponse.Match(htmlContentResult.TextContent);
#endif

        if (!match.Success || match.Groups.Count < 2)
        {
            return null;
        }

        var rawJson = match.Groups[groupnum: 1].Value;

        using var jsonDoc = JsonDocument.Parse(rawJson, new JsonDocumentOptions
        {
            AllowTrailingCommas = true
        });

        var title = "";
        var description = "";

        if (jsonDoc.RootElement.TryGetProperty(propertyName: "microformat", out var microformat) &&
            microformat.TryGetProperty(propertyName: "playerMicroformatRenderer", out var renderer))
        {
            if (renderer.TryGetProperty(propertyName: "title", out var titleNode) &&
                titleNode.TryGetProperty(propertyName: "simpleText", out var titleText))
            {
                title = titleText.GetString() ?? "";
            }

            if (renderer.TryGetProperty(propertyName: "description", out var descNode) &&
                descNode.TryGetProperty(propertyName: "simpleText", out var descText))
            {
                description = descText.GetString() ?? "";
            }
        }

        return $"{title}\n{description}".Trim();
    }

    public static async Task<YouTubeVideoResponseSnippet?> GetYoutubeVideoInfoAsync(this HttpClient httpClient,
        string videoId,
        string apiKey,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        var url = $"https://www.googleapis.com/youtube/v3/videos?id={videoId}&part=snippet&key={apiKey}";

        var responseResult = await httpClient.SafeFetchAsync(url, cancellationToken: cancellationToken);

        if (responseResult.Kind != FetchResultKind.Success || responseResult.TextContent.IsEmpty())
        {
            return null;
        }

        var data = JsonSerializer.Deserialize<YouTubeVideoResponse>(responseResult.TextContent, JsonOptions);

        return data?.Items.FirstOrDefault()?.Snippet;
    }

#if !NET_6
    [GeneratedRegex(pattern: @"var ytInitialPlayerResponse = (\{.+?\});",
        RegexOptions.Compiled | RegexOptions.Singleline, matchTimeoutMilliseconds: 3000)]
    private static partial Regex PlayerResponse();
#else
    private static readonly Regex PlayerResponse = new(pattern: @"var ytInitialPlayerResponse = (\{.+?\});",
        RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(value: 3000));
#endif
}
