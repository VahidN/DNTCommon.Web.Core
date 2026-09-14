using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core.TestConsoleApp.Services;

public static class ServicesRegistry
{
    public static IServiceCollection AddAppServices(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        serviceCollection.AddDNTCommonWeb(autoInjectAllServices: true);

        return serviceCollection;
    }
}
