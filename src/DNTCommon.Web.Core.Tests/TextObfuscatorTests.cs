using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class TextObfuscatorTests
{
    [TestMethod]
    public void ObfuscateWithHiddenMarkersShouldReturnCorrectValue()
        => Assert.IsTrue("test".ObfuscateWithHiddenMarkers().HasHiddenCharacters());
}
