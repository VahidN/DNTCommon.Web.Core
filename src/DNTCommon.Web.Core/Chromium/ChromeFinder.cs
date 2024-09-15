using Microsoft.Win32;

namespace DNTCommon.Web.Core;

/// <summary>
///     This class searches for the Chrome or Chromium executables cross-platform.
/// </summary>
public static class ChromeFinder
{
    private static void GetApplicationDirectories(List<string> directories)
    {
        if (OperatingSystem.IsWindows())
        {
            // c:\Program Files\Google\Chrome\Application\
            const string subDirectory = "Google\\Chrome\\Application";

            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                subDirectory));

            directories.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                subDirectory));
        }
        else if (OperatingSystem.IsLinux())
        {
            directories.Add(item: "/usr/local/sbin");
            directories.Add(item: "/usr/local/bin");
            directories.Add(item: "/usr/sbin");
            directories.Add(item: "/usr/bin");
            directories.Add(item: "/sbin");
            directories.Add(item: "/bin");
            directories.Add(item: "/opt/google/chrome");
        }
        else if (OperatingSystem.IsMacOS())
        {
            directories.Add(item: "/Applications");
        }
    }

    private static string GetAppPath()
    {
        var appPath = AppDomain.CurrentDomain.BaseDirectory;

        if (appPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return appPath;
        }

        return appPath + Path.DirectorySeparatorChar;
    }

    /// <summary>
    ///     Tries to find Chrome
    /// </summary>
    /// <returns></returns>
    public static string? Find()
    {
        if (OperatingSystem.IsWindows())
        {
            var key = Registry.GetValue(
                keyName:
                @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe",
                valueName: "Path", string.Empty);

            if (key != null)
            {
                var path = Path.Combine(key.ToString()!, path2: "chrome.exe");

                if (File.Exists(path))
                {
                    return path;
                }
            }
        }

        var exeNames = new List<string>();

        if (OperatingSystem.IsWindows())
        {
            exeNames.Add(item: "chrome.exe");
        }
        else if (OperatingSystem.IsLinux())
        {
            exeNames.Add(item: "google-chrome");
            exeNames.Add(item: "chrome");
            exeNames.Add(item: "chromium");
            exeNames.Add(item: "chromium-browser");
        }
        else if (OperatingSystem.IsMacOS())
        {
            exeNames.Add(item: "Google Chrome.app/Contents/MacOS/Google Chrome");
            exeNames.Add(item: "Chromium.app/Contents/MacOS/Chromium");
            exeNames.Add(item: "Google Chrome.app/Contents/MacOS/Google Chrome");
        }

        var currentPath = GetAppPath();

        foreach (var exeName in exeNames)
        {
            var path = Path.Combine(currentPath, exeName);

            if (File.Exists(path))
            {
                return path;
            }
        }

        var directories = new List<string>();

        GetApplicationDirectories(directories);

        return (from exeName in exeNames
            from directory in directories
            select Path.Combine(directory, exeName)).FirstOrDefault(File.Exists);
    }
}