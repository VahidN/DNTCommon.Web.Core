using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core.Tests;

public class TestsBase
{
    public TestsBase() => ServiceProvider = getServiceProvider();

    public IServiceProvider ServiceProvider { get; private set; }

    private static IServiceProvider getServiceProvider()
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(path: "appsettings.json", reloadOnChange: true, optional: false)
            .AddInMemoryCollection([new KeyValuePair<string, string>(key: "UseInMemoryDatabase", value: "true")])
            .Build();

        services.AddSingleton(provider => configuration);
        services.Configure<SmtpConfig>(options => configuration.GetSection(key: "SmtpConfig").Bind(options));
        services.Configure<AntiDosConfig>(options => configuration.GetSection(key: "AntiDosConfig").Bind(options));
        services.Configure<AntiXssConfig>(options => configuration.GetSection(key: "AntiXssConfig").Bind(options));

        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Trace);
        });

        services.AddDNTCommonWeb();

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider;
    }
}