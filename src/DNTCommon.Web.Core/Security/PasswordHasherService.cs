using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace DNTCommon.Web.Core
{
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

    /// <summary>
    /// PasswordHasher Service
    /// </summary>
    public interface IPasswordHasherService
    {
        /// <summary>
        /// Creates a custom hash based on SHA256CryptoServiceProvider.
        /// </summary>
        string GetSha256Hash(string input);

        /// <summary>
        /// A cryptographic random number generator
        /// </summary>
        Guid CreateCryptographicallySecureGuid();
    }

    /// <summary>
    /// PasswordHasher Service
    /// </summary>
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

        /// <summary>
        /// Creates a custom hash based on SHA256CryptoServiceProvider.
        /// </summary>
        public string GetSha256Hash(string input)
        {
            using var hashAlgorithm = new SHA256CryptoServiceProvider();
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);
            Array.Reverse(byteHash);
            return Convert.ToBase64String(byteHash);
        }

        /// <summary>
        /// A cryptographic random number generator
        /// </summary>
        public Guid CreateCryptographicallySecureGuid()
        {
            var bytes = new byte[16];
            _rand.GetBytes(bytes);
            return new Guid(bytes);
        }
    }
}