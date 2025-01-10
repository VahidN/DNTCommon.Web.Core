using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core.TestWebApp;

public class SendEmailsTask : IScheduledTask
{
    private readonly ILogger<SendEmailsTask> _logger;

    public SendEmailsTask(ILogger<SendEmailsTask> logger) => _logger = logger;

    public Task RunAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        _logger.LogInformation(message: "Running Send Emails");

        return Task.CompletedTask;
    }
}