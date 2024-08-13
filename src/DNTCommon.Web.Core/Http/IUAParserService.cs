using Microsoft.AspNetCore.Http;
using UAParser;

namespace DNTCommon.Web.Core;

/// <summary>
///     This is the updated version of the UAParser library with the latest regexes.yaml file.
/// </summary>
public interface IUAParserService
{
    /// <summary>
    ///     Returns true if the device is likely to be a spider or a bot device
    /// </summary>
    Task<bool> IsSpiderClientAsync(HttpContext? httpContext);

    /// <summary>
    ///     Returns true if the device is likely to be a spider or a bot device
    /// </summary>
    Task<bool> IsSpiderClientAsync(string userAgent);

    /// <summary>
    ///     Returns the current client's info from the parsed user-agent
    /// </summary>
    Task<ClientInfo?> GetClientInfoAsync(HttpContext? httpContext);

    /// <summary>
    ///     Returns the current client's info from the parsed user-agent
    /// </summary>
    Task<ClientInfo?> GetClientInfoAsync(string userAgent);

    /// <summary>
    ///     Gets the latest regexes.yaml file from GitHub and then uses Parser.FromYaml to parse it.
    /// </summary>
    /// <param name="regexesUrl">The latest regexes.yaml file's URL</param>
    Task<Parser> GetLatestUAParserAsync(string regexesUrl =
        "https://raw.githubusercontent.com/ua-parser/uap-core/master/regexes.yaml");
}