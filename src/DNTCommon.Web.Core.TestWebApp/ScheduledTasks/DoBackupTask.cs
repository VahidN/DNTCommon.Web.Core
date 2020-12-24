using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core.TestWebApp
{
    public class DoBackupTask : IScheduledTask
    {
        private readonly ILogger<DoBackupTask> _logger;

        public DoBackupTask(ILogger<DoBackupTask> logger)
        {
            _logger = logger;
        }

        public bool IsShuttingDown { get; set; }

        public async Task RunAsync()
        {
            if (this.IsShuttingDown)
            {
                return;
            }

            _logger.LogInformation("Running Do Backup");

            await Task.Delay(TimeSpan.FromMinutes(3));
        }
    }
}