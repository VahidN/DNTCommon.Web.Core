using System;
using System.Security.Cryptography;
using System.Text;

namespace DNTCommon.Web.Core
{
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
            using var hashAlgorithm = SHA256.Create();
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