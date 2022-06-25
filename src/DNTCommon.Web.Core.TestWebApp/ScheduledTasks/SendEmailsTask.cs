using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core.TestWebApp;

public class SendEmailsTask : IScheduledTask
{
    private readonly ILogger<SendEmailsTask> _logger;

    public SendEmailsTask(ILogger<SendEmailsTask> logger)
    {
        _logger = logger;
    }

    public bool IsShuttingDown { get; set; }

    public Task RunAsync()
    {
        if (this.IsShuttingDown)
        {
            return Task.CompletedTask;
        }

        _logger.LogInformation("Running Send Emails");

        return Task.CompletedTask;
    }
}