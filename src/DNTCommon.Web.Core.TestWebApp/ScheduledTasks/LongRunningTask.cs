using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core.TestWebApp;

public class LongRunningTask : IScheduledTask
{
    private readonly ILogger<LongRunningTask> _logger;
    public bool IsShuttingDown { get; set; }

    public LongRunningTask(ILogger<LongRunningTask> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync()
    {
        if (this.IsShuttingDown)
        {
            return;
        }

        _logger.LogInformation("Running The LongRunningTask.");

        await Task.Delay(TimeSpan.FromMinutes(3));
    }
}
