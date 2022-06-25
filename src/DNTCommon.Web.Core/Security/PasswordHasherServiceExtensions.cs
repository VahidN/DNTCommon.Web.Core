using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core;

/// <summary>
/// PasswordHasher Service Extensions
/// </summary>
public static class PasswordHasherServiceExtensions
{
    /// <summary>
    /// Adds IPasswordHasherService to IServiceCollection.
    /// </summary>
    public static IServiceCollection AddPasswordHasherService(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
        return services;
    }
}