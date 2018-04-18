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

            services.AddSingleton<IConfigurationRoot>(provider =>
            {
                return new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                                .AddInMemoryCollection(new[]
                                {
                                    new KeyValuePair<string,string>("UseInMemoryDatabase", "true"),
                                })
                                .Build();
            });

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