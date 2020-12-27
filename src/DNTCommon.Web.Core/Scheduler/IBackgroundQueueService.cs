using System;
using System.Threading;
using System.Threading.Tasks;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// A .NET Core replacement for the old HostingEnvironment.QueueBackgroundWorkItem.
    /// </summary>
    public interface IBackgroundQueueService
    {
        /// <summary>
        /// Schedules a task which can run in the background, independent of any request
        /// </summary>
        void QueueBackgroundWorkItem(Func<CancellationToken, IServiceProvider, Task> workItem);

        /// <summary>
        /// Schedules a task which can run in the background, independent of any request.
        /// </summary>
        void QueueBackgroundWorkItem(Action<CancellationToken, IServiceProvider> workItem);
    }
}