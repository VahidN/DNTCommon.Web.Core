using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DNTCommon.Web.Core.TestWebApp;

public class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(context.Configuration.GetSection(key: "Logging"));

                    logging.AddSimpleConsole(opts =>
                    {
                        opts.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffffffZ-";
                        opts.ColorBehavior = LoggerColorBehavior.Enabled;
                    });

                    logging.AddDebug();

                    logging.AddAsyncFileLogger(new FileLoggerOptions(
                        context.HostingEnvironment.WebRootPath.SafePathCombine("app-logs"),
                        secretKey: "Your-Very-Strong-Secret-Key-Here"));
                });

                webBuilder.UseDefaultServiceProvider((context, options) =>
                    {
                        options.ValidateScopes = true;
                        options.ValidateOnBuild = true;
                    })
                    .UseStartup<Startup>();
            });
}
