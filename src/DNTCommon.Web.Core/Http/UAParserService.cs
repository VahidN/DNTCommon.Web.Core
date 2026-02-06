using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UAParser;

namespace DNTCommon.Web.Core;

/// <summary>
///     This is the updated version of the UAParser library with the latest regexes.yaml file.
/// </summary>
public class UAParserService(IHttpClientFactory httpClientFactory, ILogger<UAParserService> logger) : IUAParserService
{
    private Parser? _parser;

    /// <summary>
    ///     Gets the latest regexes.yaml file from GitHub and then uses Parser.FromYaml to parse it.
    /// </summary>
    /// <param name="regexesUrl">The latest regexes.yaml file's URL</param>
    /// <param name="cancellationToken"></param>
    public async Task<Parser> GetLatestUAParserAsync(string regexesUrl =
            "https://raw.githubusercontent.com/ua-parser/uap-core/master/regexes.yaml",
        CancellationToken cancellationToken = default)
        => _parser ??= await LoadLatestParserAsync(regexesUrl, cancellationToken);

    /// <summary>
    ///     Returns the current client's info from the parsed user-agent
    /// </summary>
    public async Task<ClientInfo?>
        GetClientInfoAsync(HttpContext? httpContext, CancellationToken cancellationToken = default)
        => httpContext is null
            ? null
            : await GetClientInfoAsync(httpContext.GetUserAgent() ?? "unknown", cancellationToken);

    /// <summary>
    ///     Returns the current client's info from the parsed user-agent
    /// </summary>
    public async Task<ClientInfo?> GetClientInfoAsync(string userAgent, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(userAgent)
            ? null
            : (await GetLatestUAParserAsync(cancellationToken: cancellationToken)).Parse(userAgent);

    /// <summary>
    ///     Returns true if the device is likely to be a spider or a bot device
    /// </summary>
    public async Task<bool> IsSpiderClientAsync(string userAgent, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(userAgent) || (await GetLatestUAParserAsync(cancellationToken: cancellationToken))
            .Parse(userAgent)
            .Device.IsSpider;

    /// <summary>
    ///     Returns true if the device is likely to be a spider or a bot device
    /// </summary>
    public async Task<bool> IsSpiderClientAsync(HttpContext? httpContext, CancellationToken cancellationToken = default)
        => httpContext is null || await IsSpiderClientAsync(httpContext.GetUserAgent() ?? "unknown", cancellationToken);

    private async Task<Parser> LoadLatestParserAsync(string regexesUrl, CancellationToken cancellationToken = default)
    {
        if (!NetworkExtensions.IsConnectedToInternet(TimeSpan.FromSeconds(value: 2)))
        {
            return Parser.GetDefault();
        }

        try
        {
            using var client = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);
            var contentResult = await client.SafeFetchAsync(regexesUrl, cancellationToken: cancellationToken);

            if (contentResult.Kind != FetchResultKind.Success || contentResult.TextContent.IsEmpty())
            {
                throw new InvalidOperationException(
                    $"{regexesUrl} -> {contentResult.StatusCode} -> {contentResult.Reason}");
            }

            return Parser.FromYaml(contentResult.TextContent, new ParserOptions
            {
                UseCompiledRegex = true
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Demystify(), message: "LoadLatestRegexesYamlAsync Error");
        }

        return Parser.GetDefault();
    }
}
