using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class AesCryptoProviderTests
{
    private const string Password = "MY_Pass";
    private const string Salt = "MY_Salt";

    [TestMethod]
    public void AesEncryptShouldReturnCorrectValue()
    {
        // Arrange
        var plainText = "plainText";
        var expected = "PR_LBYYfCm8WiwBy4AGAWQ";

        // Act
        var actual = plainText.AesEncrypt(Password, Salt);

        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AesDecryptShouldReturnCorrectValue()
    {
        // Arrange
        var cipherText = "PR_LBYYfCm8WiwBy4AGAWQ";
        var expected = "plainText";

        // Act
        var actual = cipherText.AesDecrypt(Password, Salt);

        // Assert
        Assert.AreEqual(expected, actual);
    }
}
