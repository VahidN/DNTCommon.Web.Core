using System;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Scheduled Task
    /// </summary>
    public class ScheduledTaskStatus
    {
        /// <summary>
        /// Status of last run.
        /// </summary>
        public bool? IsLastRunSuccessful { get; set; }

        /// <summary>
        /// Is still running
        /// </summary>
        public bool IsRunning { set; get; }

        /// <summary>
        /// Task's last run's exception.
        /// </summary>
        public Exception? LastException { get; set; }

        /// <summary>
        /// Task's last run time.
        /// </summary>
        public DateTime? LastRun { get; set; }

        /// <summary>
        /// Priority of the task.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Run intervals logic.
        /// </summary>
        public Func<DateTime, bool> RunAt { get; set; } = default!;

        /// <summary>
        /// Instance of the task.
        /// </summary>
        public IScheduledTask? TaskInstance { set; get; }

        /// <summary>
        /// Type of the task.
        /// </summary>
        public Type TaskType { set; get; } = default!;

        /// <summary>
        /// Determines whether the specified object instances are the same instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is ScheduledTaskStatus asThis && asThis.TaskType == this.TaskType;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.TaskType.GetHashCode();
        }

        /// <summary>
        /// Gets the fully qualified name of the type, including its namespace but not its assembly.
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            return TaskType.FullName;
        }
    }
}