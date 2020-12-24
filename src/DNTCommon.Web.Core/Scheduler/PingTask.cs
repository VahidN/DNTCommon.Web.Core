using System.Threading.Tasks;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// DNTScheduler needs a ping service to keep it alive.
    /// </summary>
    public class PingTask : IScheduledTask
    {
        private readonly MySitePingClient _pingClient;

        /// <summary>
        /// DNTScheduler needs a ping service to keep it alive.
        /// </summary>
        public PingTask(MySitePingClient pingClient)
        {
            _pingClient = pingClient;
        }

        /// <summary>
        /// Is ASP.Net app domain tearing down?
        /// If set to true by the coordinator, the task should cleanup and return.
        /// </summary>
        public bool IsShuttingDown { get; set; }

        /// <summary>
        /// Scheduled task's logic.
        /// </summary>
        public async Task RunAsync()
        {
            if (this.IsShuttingDown)
            {
                return;
            }

            await _pingClient.WakeUp();
        }
    }
}