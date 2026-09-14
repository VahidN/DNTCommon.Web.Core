using DNTCommon.Web.Core;
using DNTCommon.Web.Core.TestConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Console.Error.WriteLine($"Unobserved Task Exception: {e.Exception.Message}");
    e.SetObserved();
};

AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    var exception = e.ExceptionObject as Exception;
    Console.Error.WriteLine($"Critical AppDomain Exception: {exception?.Message}");
};

var host = Host.CreateDefaultBuilder(args)
    .UseContentRoot(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory)))
    .ConfigureServices((context, services) => services.AddAppServices(context.Configuration))
    .ConfigureLogging((context, logging) =>
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
            Path.Combine(GetProjectFolder(context.HostingEnvironment), path2: "logs"), nameof(Program)));
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    await host.StartAsync();

    await host.Services.GetRequiredService<IAppRunnerService>()
        .StartAsync(args, host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping);
}
catch (OperationCanceledException)
{
    logger.LogWarning(message: "Application execution was aborted by the user (Ctrl+C).");
}
catch (Exception ex)
{
    logger.LogCritical(ex, message: "A fatal unhandled exception occurred during application execution.");
    Environment.ExitCode = 1;
}
finally
{
    logger.LogInformation(message: "Shutting down background services...");
    await host.StopAsync();
}

static string GetProjectFolder(IHostEnvironment hostEnvironment)
{
    ArgumentNullException.ThrowIfNull(hostEnvironment);

    var root = Path.GetFullPath(hostEnvironment.ContentRootPath);

    return Path.GetFullPath(Path.Combine(
        root.Split([$"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"],
            StringSplitOptions.RemoveEmptyEntries)[0], path2: "..", path3: ".."));
}
