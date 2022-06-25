using System;

namespace DNTCommon.Web.Core;

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