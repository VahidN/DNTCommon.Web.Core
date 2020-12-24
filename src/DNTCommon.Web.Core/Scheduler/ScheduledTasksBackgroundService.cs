using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// ScheduledTasks Background Service
    /// </summary>
    public class ScheduledTasksBackgroundService : BackgroundService
    {
        private readonly IScheduledTasksCoordinator _scheduledTasksCoordinator;

        /// <summary>
        /// ScheduledTasks Background Service
        /// </summary>
        public ScheduledTasksBackgroundService(IScheduledTasksCoordinator scheduledTasksCoordinator)
        {
            _scheduledTasksCoordinator = scheduledTasksCoordinator ?? throw new ArgumentNullException(nameof(scheduledTasksCoordinator));
        }

        /// <summary>
        /// This method is called when the IHostedService starts.
        /// </summary>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _scheduledTasksCoordinator.StartTasks();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduledTasksCoordinator.StopTasks();
        }
    }
}