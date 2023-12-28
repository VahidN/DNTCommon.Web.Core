using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class PasswordHasherServiceTests : TestsBase
{
    [TestMethod]
    public void TestPasswordHasherServiceWorks()
    {
        ServiceProvider.RunScopedService<IPasswordHasherService>(passwordHasherService =>
        {
            var password = "123$v*10p";
            var hash = passwordHasherService.GetPbkdf2Hash(password);
            Assert.IsTrue(passwordHasherService.IsValidPbkdf2Hash(hash, password));
        });
    }

    [TestMethod]
    public void TestPasswordHasherServiceWorksWithUnicodeData()
    {
        ServiceProvider.RunScopedService<IPasswordHasherService>(passwordHasherService =>
        {
            var password = "123$v*10pهست";
            var hash = passwordHasherService.GetPbkdf2Hash(password);
            Assert.IsTrue(passwordHasherService.IsValidPbkdf2Hash(hash, password));
        });
    }
}