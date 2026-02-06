using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

/// <summary>
///     DNTScheduler needs a ping service to keep it alive.
///     This class provides the SiteRootUrl for the PingTask.
/// </summary>
/// <remarks>
///     Pings the site's root url.
/// </remarks>
public class MySitePingClient(
    IHttpClientFactory httpClientFactory,
    IOptions<ScheduledTasksStorage> tasksStorage,
    ILogger<MySitePingClient> logger)
{
    /// <summary>
    ///     Pings the site's root url.
    /// </summary>
    public async Task WakeUpAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var siteRootUrl = tasksStorage.Value.SiteRootUrl;

            if (siteRootUrl.IsEmpty())
            {
                return;
            }

            using var httpClient = httpClientFactory.CreateClient(NamedHttpClient.BaseHttpClient);
            await httpClient.SafeFetchAsync(siteRootUrl, cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(ex.Demystify(),
                message: "Graceful Shutdown. Cancellation has been requested for this task.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(eventId: 0, ex.Demystify(), message: "Failed running the Ping task.");
        }
    }
}
