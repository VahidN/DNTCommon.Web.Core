using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core.TestWebApp;

public class DoBackupTask : IScheduledTask
{
    private readonly ILogger<DoBackupTask> _logger;

    public DoBackupTask(ILogger<DoBackupTask> logger) => _logger = logger;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        _logger.LogInformation(message: "Running Do Backup");

        await Task.Delay(TimeSpan.FromMinutes(minutes: 3), cancellationToken);
    }
}