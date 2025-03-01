using System;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class XmlUtilsTests : TestsBase
{
    [TestMethod]
    public void TestToSignedXmlDocumentStringWithRandomKeysWorks()
    {
        var license = new License
        {
            Id = Guid.NewGuid(),
            Domain = "https://www.dntips.ir/",
            Expiration = DateTime.Now.AddYears(value: 2),
            IssuedTo = "VahidN",
            Type = LicenseType.Standard
        };

        var rsaKeys = RsaKeys.CreateRandomKeyPair();
        var signedXml = license.ToSignedXmlDocumentString(rsaKeys);

        var operationResult = signedXml.DeserializeSignedXmlDocumentString<License>(rsaKeys);
        Assert.IsTrue(operationResult.IsSuccess);
        var deserializedLicense = operationResult.Result;
        Assert.AreEqual(license.IssuedTo, deserializedLicense?.IssuedTo);
    }

    [TestMethod]
    public void TestToSignedXmlDocumentStringWithCertFileKeysWorks()
    {
        var license = new License
        {
            Id = Guid.NewGuid(),
            Domain = "https://www.dntips.ir/",
            Expiration = DateTime.Now.AddYears(value: 2),
            IssuedTo = "VahidN",
            Type = LicenseType.Standard
        };

        var rsaKeys = RsaKeys.CreateKeyPairFromX509Certificate(pfxFilePath: "cert123.pfx", pfxPassword: "123");
        var signedXml = license.ToSignedXmlDocumentString(rsaKeys);

        var operationResult = signedXml.DeserializeSignedXmlDocumentString<License>(rsaKeys);
        Assert.IsTrue(operationResult.IsSuccess);
        var deserializedLicense = operationResult.Result;
        Assert.AreEqual(license.IssuedTo, deserializedLicense?.IssuedTo);
    }
}

public class License
{
    [XmlAttribute] public Guid Id { set; get; }

    public string Domain { set; get; }

    [XmlAttribute] public string IssuedTo { set; get; }

    [XmlAttribute] public DateTime Expiration { set; get; }

    [XmlAttribute] public LicenseType Type { set; get; }
}

public enum LicenseType
{
    None,
    Trial,
    Standard,
    Personal
}
