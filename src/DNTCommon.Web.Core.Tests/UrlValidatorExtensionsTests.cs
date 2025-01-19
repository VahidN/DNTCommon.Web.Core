using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class UrlValidatorExtensionsTests : TestsBase
{
    [TestMethod]
    [DataRow("part1.part2@gmail.com", "PART1PART2@GMAIL.COM")]
    [DataRow("part1...part2@gmail.com", "PART1PART2@GMAIL.COM")]
    [DataRow("pa.rt1.par.t2@gmail.com", "PART1PART2@GMAIL.COM")]
    [DataRow("p.ar.t1.pa.rt.2@gmail.com", "PART1PART2@GMAIL.COM")]
    [DataRow("part1.part2+spamsite@gmail.com", "PART1PART2@GMAIL.COM")]
    [DataRow("pa.rt1.par.t2+spam.site@gmail.com", "PART1PART2@GMAIL.COM")]
    [DataRow("test.ir", "test.ir")]
    public void TestGmailAddressWithDotsCanBeNormalized(string actual, string expected)
        => Assert.AreEqual(expected.ToLowerInvariant(), actual.FixGmailDots());
}