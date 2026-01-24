using System.Text.RegularExpressions;

namespace DNTCommon.Web.Core;

/// <summary>
///     Linux Info Provider
/// </summary>
public static class LinuxInfoProvider
{
    private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(value: 3);

    private static readonly Regex RegexLine = new(pattern: @"^dotnet-sdk-\d+\.\d+.*$",
        RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase, MatchTimeout);

    private static readonly Regex RegexVersion = new(pattern: @"\s(\d+\.\d+\.\d+)-",
        RegexOptions.Compiled | RegexOptions.IgnoreCase, MatchTimeout);

    public static LinuxWebServerInfo GetLinuxWebServerInfo()
        => new()
        {
            EnvironmentVariables = GetEnvironmentVariables(),
            AvailableSdkVersions = GetAvailableSdkVersions()
        };

    public static IList<string> GetEnvironmentVariables()
    {
        List<string> results = [];

        if (!OperatingSystem.IsLinux() || !File.Exists(path: "/etc/os-release"))
        {
            return results;
        }

        results.AddRange(File.ReadLines(path: "/etc/os-release"));

        return results;
    }

    /// <summary>
    ///     استخراج نسخه‌های کامل مثل 8.0.121 , 9.0.112
    /// </summary>
    public static IList<Version> GetAvailableSdkVersions()
    {
        if (!OperatingSystem.IsLinux())
        {
            return [];
        }

        var outputText = new ApplicationStartInfo
        {
            ProcessName = "apt",
            Arguments = "search dotnet-sdk*",
            AppPath = "apt",
            WaitForExit = TimeSpan.FromMinutes(value: 1),
            KillProcessOnStart = false
        }.ExecuteProcess();

        if (outputText.IsEmpty())
        {
            return [];
        }

        var versions = new HashSet<Version>();

        foreach (Match line in RegexLine.Matches(outputText))
        {
            var versionMatch = RegexVersion.Match(line.Value);

            if (versionMatch.Success)
            {
                var versionString = versionMatch.Groups[groupnum: 1].Value;

                if (Version.TryParse(versionString, out var version))
                {
                    versions.Add(version);
                }
            }
        }

        return [.. versions.OrderBy(v => v)];
    }
}
