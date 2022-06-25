using System;
using System.Collections.Generic;
using System.Linq;

namespace DNTCommon.Web.Core;

/// <summary>
/// Scheduled Tasks Storage
/// </summary>
public class ScheduledTasksStorage
{
    internal string SiteRootUrl { set; get; } = default!;

    /// <summary>
    /// Scheduled Tasks Storage
    /// </summary>
    public ScheduledTasksStorage()
    {
        Tasks = new HashSet<ScheduledTaskStatus>();
    }

    /// <summary>
    /// Gets the list of the scheduled tasks.
    /// </summary>
    public ISet<ScheduledTaskStatus> Tasks { get; }

    /// <summary>
    /// DNTScheduler needs a ping service to keep it alive.
    /// </summary>
    public void AddPingTask(string siteRootUrl)
    {
        SiteRootUrl = siteRootUrl;
    }

    /// <summary>
    /// Adds a new scheduled task.
    /// You should register this task using `services.AddTransient` method at startup class too.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="runAt">
    /// If the RunAt method returns true, the Run method will be executed.
    /// utcNow is expressed as the Coordinated Universal Time (UTC).
    /// It will be called every one second.
    /// </param>
    /// <param name="order">
    /// The priority of the task.
    /// If you have multiple jobs at the same time, this value indicates the order of their execution.
    /// </param>
    public void AddScheduledTask<T>(Func<DateTime, bool> runAt, int order = 1) where T : IScheduledTask
    {
        Tasks.Add(new ScheduledTaskStatus
        {
            TaskType = typeof(T),
            RunAt = runAt,
            Order = order
        });
    }

    /// <summary>
    /// Removes a task from the list.
    /// </summary>
    /// <exception cref="InvalidOperationException">When the T is not found.</exception>
    public void RemoveScheduledTask<T>() where T : IScheduledTask
    {
        var task = Tasks.FirstOrDefault(taskTemplate => taskTemplate.TaskType == typeof(T));
        if (task == null)
        {
            throw new InvalidOperationException($"{typeof(T)} not found and is not registered.");
        }
        Tasks.Remove(task);
    }
}