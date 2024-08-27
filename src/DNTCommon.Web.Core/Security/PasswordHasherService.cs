namespace DNTCommon.Web.Core;

/// <summary>
///     PasswordHasher Service
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

    /// <summary>
    ///     Creates a custom hash based on SHA256CryptoServiceProvider.
    /// </summary>
    public string GetSha256Hash(string input) => input.GetSha256Hash();

    /// <summary>
    ///     Creates a custom hash based on SHA1 CryptoServiceProvider.
    /// </summary>
    public string GetSha1Hash(string input) => input.GetSha1Hash();

    /// <summary>
    ///     A cryptographic random number generator
    /// </summary>
    public Guid CreateCryptographicallySecureGuid() => new(GenerateRandomSalt());

    /// <summary>
    ///     A cryptographic random number generator
    /// </summary>
    /// <returns></returns>
    public byte[] GenerateRandomSalt()
    {
        var salt = new byte[16];
        _rand.GetBytes(salt);

        return salt;
    }

    /// <summary>
    ///     Computes a salted Pbkdf2 hash. Use IsValidPbkdf2Hash method to validate it.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public string GetPbkdf2Hash(string password) => password.GetPbkdf2Hash();

    /// <summary>
    ///     Verifies a hash created by GetPbkdf2Hash method.
    /// </summary>
    /// <param name="hashedPassword"></param>
    /// <param name="providedPassword"></param>
    /// <returns></returns>
    public bool IsValidPbkdf2Hash(string hashedPassword, string providedPassword)
        => hashedPassword.IsValidPbkdf2Hash(providedPassword);
}