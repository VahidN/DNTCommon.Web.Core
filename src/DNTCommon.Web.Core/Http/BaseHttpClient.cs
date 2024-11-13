namespace DNTCommon.Web.Core;

/// <summary>
///     A typed HttpClient
/// </summary>
/// <remarks>
///     A typed HttpClient
/// </remarks>
/// <param name="httpClient"></param>
public class BaseHttpClient(HttpClient httpClient)
{
    /// <summary>
    ///     A typed HttpClient
    /// </summary>
    public HttpClient HttpClient { get; } = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
}