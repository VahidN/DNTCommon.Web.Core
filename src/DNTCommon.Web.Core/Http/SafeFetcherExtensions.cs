namespace DNTCommon.Web.Core;

public static class SafeFetcherExtensions
{
    private const int MaxRedirects = 5;

    public static async Task<FetchResult> SafeFetchAsync(this HttpClient httpClient,
        string uri,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(uri);

        return await httpClient.SafeFetchAsync(new Uri(uri), ct);
    }

    public static async Task<FetchResult> SafeFetchAsync(this HttpClient httpClient,
        Uri uri,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(uri);

        return await FetchInternalAsync(httpClient, uri, redirectCount: 0, ct);
    }

    private static async Task<FetchResult> FetchInternalAsync(HttpClient httpClient,
        Uri uri,
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
                return new FetchResult(FetchResultKind.Redirected, uri, response.StatusCode, Content: null,
                    Reason: "Too many redirects");
            }

            var location = response.Headers.Location;

            if (location == null)
            {
                return new FetchResult(FetchResultKind.Redirected, uri, response.StatusCode, Content: null,
                    Reason: "Redirect without Location header");
            }

            var finalUri = location.IsAbsoluteUri ? location : new Uri(uri, location);

            if (IsTrapRedirect(uri, finalUri))
            {
                return new FetchResult(FetchResultKind.Blocked, finalUri, response.StatusCode, Content: null,
                    Reason: "Redirect trap detected");
            }

            await Task.Delay(RandomNumberGenerator.GetInt32(fromInclusive: 500, toExclusive: 1500), ct);

            return await FetchInternalAsync(httpClient, finalUri, redirectCount + 1, ct);
        }

        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
        {
            return new FetchResult(FetchResultKind.Blocked, uri, response.StatusCode, Content: null,
                Reason: "403 / 429");
        }

        if (IsLoginOrChallenge(response))
        {
            return new FetchResult(FetchResultKind.Challenge, uri, response.StatusCode, Content: null,
                Reason: "Login or bot challenge");
        }

        if (!response.IsSuccessStatusCode)
        {
            return new FetchResult(FetchResultKind.Failed, uri, response.StatusCode, Content: null,
                Reason: "Non-success status");
        }

        var content = await response.Content.ReadAsStringAsync(ct);

        return new FetchResult(FetchResultKind.Success, uri, response.StatusCode, content);
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

    private static bool IsLoginOrChallenge(HttpResponseMessage response)
        => response.Headers.TryGetValues(name: "Set-Cookie", out var cookies) && cookies.Any(c
            => c.Contains(value: "cf_clearance", StringComparison.OrdinalIgnoreCase));
}
