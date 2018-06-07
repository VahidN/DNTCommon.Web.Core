using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTCommon.Web.Core.Tests
{
    public class TestsBase
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public TestsBase()
        {
            ServiceProvider = getServiceProvider();
        }

        private static IServiceProvider getServiceProvider()
        {
            var services = new ServiceCollection();

            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                                            .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
                                            .AddInMemoryCollection(new[]
                                            {
                                                new KeyValuePair<string,string>("UseInMemoryDatabase", "true"),
                                            })
                                            .Build();
            services.AddSingleton<IConfigurationRoot>(provider => configuration);
            services.Configure<SmtpConfig>(options => configuration.GetSection("SmtpConfig").Bind(options));
            services.Configure<AntiDosConfig>(options => configuration.GetSection("AntiDosConfig").Bind(options));
            services.Configure<AntiXssConfig>(options => configuration.GetSection("AntiXssConfig").Bind(options));

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
}