using System.IO;
using System.Threading.Tasks;

namespace DNTCommon.Web.Core.TestWebApp;

public class ExceptionalTask(IHttpRequestInfoService httpRequestInfoService) : IScheduledTask
{
    public bool IsShuttingDown { get; set; }

    public Task RunAsync()
    {
        var ip = httpRequestInfoService.GetIP() ?? "::1";

        throw new FileNotFoundException($"Couldn't find the xyz.abc file. Your IP: {ip}");
    }
}