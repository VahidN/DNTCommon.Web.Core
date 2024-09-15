using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UAParser;

namespace DNTCommon.Web.Core;

/// <summary>
///     This is the updated version of the UAParser library with the latest regexes.yaml file.
/// </summary>
public class UAParserService(BaseHttpClient baseHttpClient, ILogger<UAParserService> logger) : IUAParserService
{
    private Parser? _parser;

    /// <summary>
    ///     Gets the latest regexes.yaml file from GitHub and then uses Parser.FromYaml to parse it.
    /// </summary>
    /// <param name="regexesUrl">The latest regexes.yaml file's URL</param>
    public async Task<Parser> GetLatestUAParserAsync(string regexesUrl =
        "https://raw.githubusercontent.com/ua-parser/uap-core/master/regexes.yaml")
        => _parser ??= await LoadLatestParserAsync(regexesUrl);

    /// <summary>
    ///     Returns the current client's info from the parsed user-agent
    /// </summary>
    public async Task<ClientInfo?> GetClientInfoAsync(HttpContext? httpContext)
        => httpContext is null ? null : await GetClientInfoAsync(httpContext.GetUserAgent() ?? "unknown");

    /// <summary>
    ///     Returns the current client's info from the parsed user-agent
    /// </summary>
    public async Task<ClientInfo?> GetClientInfoAsync(string userAgent)
        => string.IsNullOrWhiteSpace(userAgent) ? null : (await GetLatestUAParserAsync()).Parse(userAgent);

    /// <summary>
    ///     Returns true if the device is likely to be a spider or a bot device
    /// </summary>
    public async Task<bool> IsSpiderClientAsync(string userAgent)
        => string.IsNullOrWhiteSpace(userAgent) || (await GetLatestUAParserAsync()).Parse(userAgent).Device.IsSpider;

    /// <summary>
    ///     Returns true if the device is likely to be a spider or a bot device
    /// </summary>
    public async Task<bool> IsSpiderClientAsync(HttpContext? httpContext)
        => httpContext is null || await IsSpiderClientAsync(httpContext.GetUserAgent() ?? "unknown");

    private async Task<Parser> LoadLatestParserAsync(string regexesUrl)
    {
        if (!NetworkExtensions.IsConnectedToInternet())
        {
            return Parser.GetDefault();
        }

        try
        {
            var content = await baseHttpClient.HttpClient.GetStringAsync(regexesUrl);

            if (!string.IsNullOrWhiteSpace(content))
            {
                return Parser.FromYaml(content, new ParserOptions
                {
                    UseCompiledRegex = true
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Demystify(), message: "LoadLatestRegexesYamlAsync Error");
        }

        return Parser.GetDefault();
    }
}