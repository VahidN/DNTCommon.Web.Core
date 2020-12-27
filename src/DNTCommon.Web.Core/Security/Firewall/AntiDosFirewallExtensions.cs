using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
    /// <summary>
    /// Anti Dos Firewall Extensions
    /// </summary>
    public static class AntiDosFirewallExtensions
    {
        /// <summary>
        /// Adds IAntiDosFirewall to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddAntiDosFirewall(this IServiceCollection services)
        {
            services.AddScoped<IAntiDosFirewall, AntiDosFirewall>();
            return services;
        }
    }
}