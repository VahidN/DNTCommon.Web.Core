using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core.TestWebApp;

public class LongRunningTask : IScheduledTask
{
    private readonly ILogger<LongRunningTask> _logger;

    public LongRunningTask(ILogger<LongRunningTask> logger) => _logger = logger;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        _logger.LogInformation(message: "Running The LongRunningTask.");

        await Task.Delay(TimeSpan.FromMinutes(minutes: 3), cancellationToken);
    }
}