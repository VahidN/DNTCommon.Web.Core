namespace DNTCommon.Web.Core;

public static class SafeFetcherExtensions
{
    private const int MaxRedirects = 5;

    public static async Task<FetchResult> SafeFetchAsync(this HttpClient httpClient,
        string uri,
        bool downloadBinaryContent = false,
        int maxAllowedSizeContentInBytes = 15 * 1024 * 1024,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(uri);

        return await httpClient.SafeFetchAsync(new Uri(uri), downloadBinaryContent, maxAllowedSizeContentInBytes,
            cancellationToken);
    }

    public static async Task<FetchResult> SafeFetchAsync(this HttpClient httpClient,
        Uri uri,
        bool downloadBinaryContent = false,
        int maxAllowedSizeContentInBytes = 15 * 1024 * 1024,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(uri);

        return await FetchInternalAsync(httpClient, uri, downloadBinaryContent, maxAllowedSizeContentInBytes,
            redirectCount: 0, cancellationToken);
    }

    private static async Task<FetchResult> FetchInternalAsync(HttpClient httpClient,
        Uri uri,
        bool downloadBinaryContent,
        int maxAllowedSizeContentInBytes,
        int redirectCount,
        CancellationToken ct)
    {
        httpClient.DefaultRequestHeaders.Referrer = uri;
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        if (response.StatusCode.IsRedirect)
        {
            if (redirectCount >= MaxRedirects)
            {
                return new FetchResult(FetchResultKind.Redirected, uri, response.StatusCode,
                    Reason: "Too many redirects");
            }

            var location = response.Headers.Location;

            if (location == null)
            {
                return new FetchResult(FetchResultKind.Redirected, uri, response.StatusCode,
                    Reason: "Redirect without Location header");
            }

            var finalUri = location.IsAbsoluteUri ? location : new Uri(uri, location);

            if (IsTrapRedirect(uri, finalUri))
            {
                return new FetchResult(FetchResultKind.Blocked, finalUri, response.StatusCode,
                    Reason: "Redirect trap detected");
            }

            await Task.Delay(RandomNumberGenerator.GetInt32(fromInclusive: 500, toExclusive: 1500), ct);

            return await FetchInternalAsync(httpClient, finalUri, downloadBinaryContent, maxAllowedSizeContentInBytes,
                redirectCount + 1, ct);
        }

        var contentType = response.Content.Headers.ContentType?.MediaType;

        if (response.StatusCode == HttpStatusCode.Forbidden && IsCloudflare(response))
        {
            return new FetchResult(FetchResultKind.Challenge, uri, response.StatusCode, ContentType: contentType,
                Reason: "Cloudflare protection");
        }

        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
        {
            return new FetchResult(FetchResultKind.Blocked, uri, response.StatusCode, ContentType: contentType,
                Reason: "403 / 429");
        }

        if (IsLoginOrChallenge(response))
        {
            return new FetchResult(FetchResultKind.Challenge, uri, response.StatusCode, ContentType: contentType,
                Reason: "Login or bot challenge");
        }

        if (!response.IsSuccessStatusCode)
        {
            return new FetchResult(FetchResultKind.Failed, uri, response.StatusCode, ContentType: contentType,
                Reason: "Non-success status");
        }

        var length = response.Content.Headers.ContentLength;

        if (length > maxAllowedSizeContentInBytes)
        {
            return new FetchResult(FetchResultKind.Failed, uri, response.StatusCode, ContentType: contentType,
                Reason: string.Create(CultureInfo.InvariantCulture, $"File is too large ({length} bytes)"));
        }

        if (IsBinaryContent(contentType))
        {
            if (!downloadBinaryContent)
            {
                return new FetchResult(FetchResultKind.Failed, uri, response.StatusCode, ContentType: contentType,
                    Reason: "Binary file detected – download skipped");
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(ct);

            return new FetchResult(FetchResultKind.Success, uri, response.StatusCode, BinaryContent: bytes,
                ContentType: contentType);
        }

        var text = await response.Content.ReadAsStringAsync(ct);

        return new FetchResult(FetchResultKind.Success, uri, response.StatusCode, text, ContentType: contentType);
    }

    private static bool IsTrapRedirect(Uri from, Uri to)
    {
        // host change!
        if (!string.Equals(from.Host, to.Host, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var path = to.AbsolutePath.ToLowerInvariant();

        return path.Contains(value: "login", StringComparison.OrdinalIgnoreCase) ||
               path.Contains(value: "signin", StringComparison.OrdinalIgnoreCase) ||
               path.Contains(value: "challenge", StringComparison.OrdinalIgnoreCase) ||
               path.Contains(value: "auth", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCloudflare(HttpResponseMessage response)
        => response.Headers.Server?.ToString()?.Contains(value: "cloudflare", StringComparison.OrdinalIgnoreCase) ==
           true || response.Headers.TryGetValues(name: "CF-RAY", out _) ||
           response.Headers.TryGetValues(name: "CF-Cache-Status", out _);

    private static bool IsLoginOrChallenge(HttpResponseMessage response)
        => response.Headers.TryGetValues(name: "Set-Cookie", out var cookies) && cookies.Any(c
            => c.Contains(value: "cf_clearance", StringComparison.OrdinalIgnoreCase));

    private static bool IsBinaryContent(string? contentType)
    {
        if (contentType.IsEmpty())
        {
            return false;
        }

        return !contentType.StartsWith(value: "text/", StringComparison.OrdinalIgnoreCase) &&
               !string.Equals(contentType, b: "application/json", StringComparison.OrdinalIgnoreCase) &&
               !string.Equals(contentType, b: "application/xml", StringComparison.OrdinalIgnoreCase) &&
               !string.Equals(contentType, b: "application/xhtml+xml", StringComparison.OrdinalIgnoreCase);
    }
}
