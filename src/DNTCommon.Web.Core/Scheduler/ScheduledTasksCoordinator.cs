using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using static System.FormattableString;
using System.Diagnostics;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Scheduled Tasks Manager
    /// </summary>
    public sealed class ScheduledTasksCoordinator : IScheduledTasksCoordinator
    {
        // the 30 seconds is for the entire app to tie up what it's doing.
        private const int TimeToFinish = 30 * 1000;

        private readonly IJobsRunnerTimer _jobsRunnerTimer;
        private readonly ILogger<ScheduledTasksCoordinator> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<ScheduledTasksStorage> _tasksStorage;
        private bool _isShuttingDown;

        /// <summary>
        /// Scheduled Tasks Manager
        /// </summary>
        public ScheduledTasksCoordinator(
            ILogger<ScheduledTasksCoordinator> logger,
            IHostApplicationLifetime applicationLifetime,
            IOptions<ScheduledTasksStorage> tasksStorage,
            IJobsRunnerTimer jobsRunnerTimer,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tasksStorage = tasksStorage ?? throw new ArgumentNullException(nameof(tasksStorage));
            _jobsRunnerTimer = jobsRunnerTimer ?? throw new ArgumentNullException(nameof(jobsRunnerTimer));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            if (applicationLifetime == null)
            {
                throw new ArgumentNullException(nameof(applicationLifetime));
            }

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                _logger.LogInformation("Application is stopping ... .");
                disposeResources().Wait();
            });
        }

        /// <summary>
        /// Starts the scheduler.
        /// </summary>
        public void StartTasks()
        {
            if (_jobsRunnerTimer.IsRunning)
            {
                return;
            }

            _jobsRunnerTimer.OnThreadPoolTimerCallback = () =>
            {
                var now = DateTime.UtcNow;

                var tasks = new List<Task>();
                foreach (var taskStatus in _tasksStorage.Value.Tasks
                                                                .Where(x => x.RunAt(now))
                                                                .OrderBy(x => x.Order))
                {
                    if (_isShuttingDown)
                    {
                        return;
                    }

                    if (taskStatus.IsRunning)
                    {
                        _logger.LogInformation($"Ignoring `{taskStatus}` task. It's still running.");
                        continue;
                    }

                    tasks.Add(Task.Run(() => runTask(taskStatus, now)));
                }

                if (tasks.Any())
                {
                    Task.WaitAll(tasks.ToArray());
                }
            };
            _jobsRunnerTimer.StartJobs();
        }

        /// <summary>
        /// Stops the scheduler.
        /// </summary>
        public Task StopTasks()
        {
            return disposeResources();
        }

        private async Task disposeResources()
        {
            if (_isShuttingDown)
            {
                return;
            }

            try
            {
                _isShuttingDown = true;

                foreach (var task in _tasksStorage.Value.Tasks.Where(x => x.TaskInstance != null))
                {
                    if (task.TaskInstance == null)
                    {
                        continue;
                    }

                    task.TaskInstance.IsShuttingDown = true;
                }

                var timeOut = TimeToFinish;
                while (_tasksStorage.Value.Tasks.Any(x => x.IsRunning) && timeOut >= 0)
                {
                    // still running ...
                    await Task.Delay(50);
                    timeOut -= 50;
                }
            }
            finally
            {
                _jobsRunnerTimer.StopJobs();
                await wakeUp();
            }
        }

        private async Task wakeUp()
        {
            var mySitePingClient = _serviceProvider.GetService<MySitePingClient>();
            if (mySitePingClient != null)
            {
                await mySitePingClient.WakeUp();
            }
        }

        private void runTask(ScheduledTaskStatus taskStatus, DateTime now)
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scheduledTask = (IScheduledTask)serviceScope.ServiceProvider.GetRequiredService(taskStatus.TaskType);
                taskStatus.TaskInstance = scheduledTask;
                var name = scheduledTask.GetType().GetTypeInfo().FullName;

                try
                {
                    if (_isShuttingDown)
                    {
                        return;
                    }

                    taskStatus.IsRunning = true;
                    taskStatus.LastRun = now;

                    _logger.LogInformation(Invariant($"Start running `{name}` task @ {now}."));
                    scheduledTask.RunAsync().Wait();

                    _logger.LogInformation(Invariant($"Finished running `{name}` task @ {now}."));
                    taskStatus.IsLastRunSuccessful = true;
                }
                catch (Exception ex)
                {
                    var exception = ex.Demystify();
                    _logger.LogCritical(0, exception, $"Failed running {name}");
                    taskStatus.IsLastRunSuccessful = false;
                    taskStatus.LastException = exception;
                }
                finally
                {
                    taskStatus.IsRunning = false;
                    taskStatus.TaskInstance = null;
                }
            }
        }
    }
}