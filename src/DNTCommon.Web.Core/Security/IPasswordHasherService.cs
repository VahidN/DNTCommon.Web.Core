namespace DNTCommon.Web.Core;

/// <summary>
///     PasswordHasher Service
/// </summary>
public interface IPasswordHasherService
{
    /// <summary>
    ///     Creates a custom hash based on SHA256CryptoServiceProvider.
    /// </summary>
    string GetSha256Hash(string input);

    /// <summary>
    ///     Creates a custom hash based on SHA1 CryptoServiceProvider.
    /// </summary>
    string GetSha1Hash(string input);

    /// <summary>
    ///     Computes a salted Pbkdf2 hash. Use IsValidPbkdf2Hash method to validate it.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    string GetPbkdf2Hash(string password);

    /// <summary>
    ///     Verifies a hash created by GetPbkdf2Hash method.
    /// </summary>
    /// <param name="hashedPassword"></param>
    /// <param name="providedPassword"></param>
    /// <returns></returns>
    bool IsValidPbkdf2Hash(string hashedPassword, string providedPassword);

    /// <summary>
    ///     A cryptographic random number generator
    /// </summary>
    /// <returns></returns>
    byte[] GenerateRandomSalt();

    /// <summary>
    ///     A cryptographic random number generator
    /// </summary>
    Guid CreateCryptographicallySecureGuid();
}