using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DNTCommon.Web.Core.TestWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseDefaultServiceProvider((context, options) =>
                                {
                                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                                    options.ValidateOnBuild = true;
                                })
                        .UseStartup<Startup>();
                });
    }
}