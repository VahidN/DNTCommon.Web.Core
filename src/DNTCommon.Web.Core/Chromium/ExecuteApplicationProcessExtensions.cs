using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     A helper class to execute the Chrome's process
/// </summary>
public static class ExecuteApplicationProcessExtensions
{
    private static readonly TimeSpan DefaultWaitForExit = TimeSpan.FromMinutes(value: 3);

    /// <summary>
    ///     A helper method to execute a process
    /// </summary>
    /// <remarks>
    ///     If the process does not exit within the specified timeout, it will be forcefully killed.
    /// </remarks>
    public static async Task<ExecuteProcessInfo> ExecuteProcessAsync(this ApplicationStartInfo startInfo,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        KillAllAppProcesses(startInfo);

        var output = new StringBuilder();

        using var process = startInfo.CreateProcess();
        SetupProcessEventHandlers(process, output);

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            if (startInfo.WaitForExit.HasValue)
            {
                using var cts = startInfo.WaitForExit.Value.ToCancellationTokenSource();
                await process.WaitForExitAsync(cts.Token);
            }
            else
            {
                await process.WaitForExitAsync(cancellationToken);
            }
        }
        finally
        {
            TeardownProcessEventHandlers(process, output);
        }

        return GetProcessOutputData(output.ToString().Trim(), process);
    }

    /// <summary>
    ///     A helper method to execute a process
    /// </summary>
    /// <remarks>
    ///     If the process does not exit within the specified timeout, it will be forcefully killed.
    /// </remarks>
    public static ExecuteProcessInfo ExecuteProcess(this ApplicationStartInfo startInfo)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        KillAllAppProcesses(startInfo);

        var output = new StringBuilder();

        using var process = startInfo.CreateProcess();
        SetupProcessEventHandlers(process, output);

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var waitTimeMs = (int)(startInfo.WaitForExit?.TotalMilliseconds ?? DefaultWaitForExit.TotalMilliseconds);
            process.WaitForExit(waitTimeMs);
        }
        finally
        {
            TeardownProcessEventHandlers(process, output);
        }

        return GetProcessOutputData(output.ToString().Trim(), process);
    }

    private static void SetupProcessEventHandlers(Process process, StringBuilder output)
    {
        void OnProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) => output.AppendLine(e.Data);

        process.OutputDataReceived += OnProcessOnOutputDataReceived;
        process.ErrorDataReceived += OnProcessOnOutputDataReceived;
    }

    private static void TeardownProcessEventHandlers(Process process, StringBuilder output)
    {
        void OnProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e) => output.AppendLine(e.Data);

        process.OutputDataReceived -= OnProcessOnOutputDataReceived;
        process.ErrorDataReceived -= OnProcessOnOutputDataReceived;
    }

    private static ExecuteProcessInfo GetProcessOutputData(string outputMessage, Process process)
    {
        var (exitCode, isExited) = GetProcessExitInfo(process);

        if (!isExited)
        {
            KillThisProcess(process);
        }

        return new ExecuteProcessInfo(exitCode, isExited, outputMessage);
    }

    private static void KillThisProcess(Process process)
    {
        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch (Exception ex)
        {
            // Process may have already exited or access denied - not critical for operation
            Debug.WriteLine($"Failed to kill process: {ex.Message}");
        }
    }

    private static void KillAllAppProcesses(ApplicationStartInfo startInfo)
    {
        if (!startInfo.KillProcessOnStart || startInfo.ProcessName.IsEmpty())
        {
            return;
        }

        try
        {
            var processes = Process.GetProcessesByName(startInfo.ProcessName);

            foreach (var process in processes)
            {
                KillThisProcess(process);
            }
        }
        catch (Exception ex)
        {
            // Failed to retrieve or kill processes - not critical for operation
            Debug.WriteLine($"Failed to kill processes: {ex.Message}");
        }
    }

    private static (int ExitCode, bool IsExited) GetProcessExitInfo(Process process)
    {
        try
        {
            return (process.ExitCode, process.HasExited);
        }
        catch (Exception ex)
        {
            // Process may have already exited - not critical, return default values
            Debug.WriteLine($"Failed to get process exit info: {ex.Message}");
        }

        return (-1, false);
    }

    private static Process CreateProcess(this ApplicationStartInfo startInfo)
    {
        var process = new Process();

        process.StartInfo.UseShellExecute = false;

        if (!startInfo.AppPath.IsEmpty())
        {
            process.StartInfo.FileName = startInfo.AppPath;
        }
        else if (!startInfo.ProcessName.IsEmpty())
        {
            process.StartInfo.FileName = startInfo.ProcessName;

            // On Linux/Unix systems with UseShellExecute = false, the system cannot find the executable
            // using just the process name from the PATH environment variable. It needs either:
            // The full absolute path to the executable, or
            // UseShellExecute = true to let the shell resolve the command from PATH
            process.StartInfo.UseShellExecute = OperatingSystem.IsLinux() || OperatingSystem.IsMacOS();
        }

        if (!startInfo.Arguments.IsEmpty())
        {
            process.StartInfo.Arguments = startInfo.Arguments;
        }

        if (!startInfo.ArgumentsList.IsNullOrEmpty())
        {
            foreach (var arg in startInfo.ArgumentsList)
            {
                process.StartInfo.ArgumentList.Add(arg);
            }
        }

        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardInput = true;

        if (!startInfo.WorkingDirectory.IsEmpty())
        {
            process.StartInfo.WorkingDirectory = startInfo.WorkingDirectory;
        }

        return process;
    }
}
