namespace DNTCommon.Web.Core.TestConsoleApp.Services;

public interface IAppRunnerService : ISingletonService
{
    Task<bool> StartAsync(string[] args, CancellationToken cancellationToken);
}
