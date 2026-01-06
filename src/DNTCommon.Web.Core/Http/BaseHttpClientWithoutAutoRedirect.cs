namespace DNTCommon.Web.Core;

/// <summary>
///     A typed HttpClient
/// </summary>
/// <param name="httpClient"></param>
public class BaseHttpClientWithoutAutoRedirect(HttpClient httpClient)
{
    /// <summary>
    ///     A typed HttpClient
    /// </summary>
    public HttpClient HttpClient { get; } = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
}
