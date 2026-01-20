using System.Runtime.InteropServices;

namespace DNTCommon.Web.Core;

/// <summary>
///     DotNet Info Provider
/// </summary>
public static class DotNetInfoProvider
{
    /// <summary>
    ///     Provides info about the web server's runtime
    /// </summary>
    public static WebServerRuntime GetWebServerRuntime()
        => new()
        {
            InformationalVersion =
                Assembly.GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion,
            SdkCheckInfo = GetSdkCheckInfo(),
            DotNetInfo = GetDotNetInfo(),
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier
        };

    /// <summary>
    ///     Returns output of `dotnet sdk check`
    /// </summary>
    public static string GetSdkCheckInfo()
        => new ApplicationStartInfo
        {
            ProcessName = "dotnet",
            Arguments = "sdk check",
            AppPath = "dotnet",
            WaitForExit = TimeSpan.FromSeconds(value: 3),
            KillProcessOnStart = false
        }.ExecuteProcess();

    /// <summary>
    ///     Returns output of `dotnet --info`
    /// </summary>
    public static string GetDotNetInfo()
        => new ApplicationStartInfo
        {
            ProcessName = "dotnet",
            Arguments = "--info",
            AppPath = "dotnet",
            WaitForExit = TimeSpan.FromSeconds(value: 3),
            KillProcessOnStart = false
        }.ExecuteProcess();

    /// <summary>
    ///     Returns output of `dotnet sdk check`
    ///     https://github.com/dotnet/sdk/blob/a34f1ca17979f6cb283ad74c53ca68f8575fceb9/src/Cli/dotnet/commands/dotnet-sdk/check/LocalizableStrings.resx
    /// </summary>
    public static (bool Success, string Info) IsNewSdkVersionAvailable()
    {
        var info = GetSdkCheckInfo();

        return (info.Contains(value: "is available", StringComparison.OrdinalIgnoreCase) ||
               info.Contains(value: "patch", StringComparison.OrdinalIgnoreCase) || (!OperatingSystem.IsLinux() &&
                   info.Contains(value: "newest", StringComparison.OrdinalIgnoreCase)), info);
    }
}
