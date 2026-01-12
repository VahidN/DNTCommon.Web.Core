using System.Runtime.InteropServices;

namespace DNTCommon.Web.Core;

/// <summary>
///     Application Process Info Provider
/// </summary>
public static class ApplicationProcessInfoProvider
{
    /// <summary>
    ///     Provides info about the web server's application
    /// </summary>
    /// <returns></returns>
    public static ApplicationProcess GetApplicationProcess()
    {
        var process = Process.GetCurrentProcess();

        return new ApplicationProcess
        {
            ProcessArguments = string.Join(separator: ' ', Environment.GetCommandLineArgs()),
            ProcessPath = Environment.ProcessPath ?? "",
            ProcessName = process.MainModule?.ModuleName ?? process.ProcessName,
            ProcessId = process.Id.ToString(CultureInfo.InvariantCulture),
            ProcessStartTime = process.StartTime,
            ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString()
        };
    }
}
