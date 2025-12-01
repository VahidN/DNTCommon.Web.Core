using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core;

/// <summary>
///     DNTScheduler needs a ping service to keep it alive.
///     This class provides the SiteRootUrl for the PingTask.
/// </summary>
/// <remarks>
///     Pings the site's root url.
/// </remarks>
public class MySitePingClient(HttpClient httpClient, ILogger<MySitePingClient> logger)
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<MySitePingClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    ///     Pings the site's root url.
    /// </summary>
    public async Task WakeUpAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_httpClient.BaseAddress is not null)
            {
                await _httpClient.GetStringAsync(_httpClient.BaseAddress, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(eventId: 0, ex.Demystify(), message: "Failed running the Ping task.");
        }
    }
}
