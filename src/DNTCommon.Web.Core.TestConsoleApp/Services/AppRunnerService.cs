using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core.TestConsoleApp.Services;

/// <summary>
///     Defines the entry point of the application
/// </summary>
public partial class AppRunnerService(ILogger<AppRunnerService> logger) : IAppRunnerService
{
    public async Task<bool> StartAsync(string[] args, CancellationToken cancellationToken)
    {
        Console.WriteLine(value: "Testing... MyApp is executing.");
        LogProvidedCommandLineArguments();

        await Task.Delay(TimeSpan.FromSeconds(seconds: 1), cancellationToken);
        Console.WriteLine(value: "Testing... MyApp finished its short delay.");

        return false;
    }

    [LoggerMessage(LogLevel.Error,
        message:
        "{CallerMemberName}: Check the provided command line arguments. It should contain -t 'arg1' -i 'arg2' -k 'arg3'")]
    partial void LogProvidedCommandLineArguments([CallerMemberName] string? callerMemberName = null);
}
