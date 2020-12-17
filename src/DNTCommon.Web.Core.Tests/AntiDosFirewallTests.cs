using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests
{
    [TestClass]
    public class AntiDosFirewallTests : TestsBase
    {
        [TestMethod]
        public void TestIsGoodBotMethodWorks()
        {
            ServiceProvider.RunScopedService<IAntiDosFirewall>(antiDosFirewall =>
            {
                Assert.IsTrue(antiDosFirewall.IsGoodBot(new AntiDosFirewallRequestInfo
                {
                    IP = "192.168.1.1",
                    UserAgent = "google",
                    UrlReferrer = new Uri("https://google.com"),
                    RawUrl = "http://localhost:5000/home",
                    IsLocal = true
                }));
            });
        }

        [TestMethod]
        public void TestIsBadBotMethodWorks()
        {
            ServiceProvider.RunScopedService<IAntiDosFirewall>(antiDosFirewall =>
                        {
                            Assert.IsTrue(antiDosFirewall.IsBadBot(new AntiDosFirewallRequestInfo
                            {
                                IP = "192.168.1.1",
                                UserAgent = "asafaweb",
                                UrlReferrer = new Uri("https://google.com"),
                                RawUrl = "http://localhost:5000/home",
                                IsLocal = true
                            }).ShouldBlockClient);
                        });
        }

        [TestMethod]
        public void TestHasUrlAttackVectorsMethodWorks()
        {
            ServiceProvider.RunScopedService<IAntiDosFirewall>(antiDosFirewall =>
                        {
                            Assert.IsTrue(antiDosFirewall.HasUrlAttackVectors(new AntiDosFirewallRequestInfo
                            {
                                IP = "192.168.1.1",
                                UserAgent = "google",
                                UrlReferrer = new Uri("https://google.com"),
                                RawUrl = "http://localhost:5000/home/.svn/",
                                IsLocal = true
                            }).ShouldBlockClient);
                        });
        }

        [TestMethod]
        public void TestShouldBanBotHeadersMethodWorks()
        {
            ServiceProvider.RunScopedService<IAntiDosFirewall>(antiDosFirewall =>
                        {
                            Assert.IsTrue(antiDosFirewall.ShouldBanBotHeaders(new AntiDosFirewallRequestInfo
                            (new HeaderDictionary(new Dictionary<string, StringValues>
                                {
                                    {"ACUNETIX",  "in use"}
                                }))
                            {
                                IP = "192.168.1.1",
                                UserAgent = "google",
                                UrlReferrer = new Uri("https://google.com"),
                                RawUrl = "http://localhost:5000/home",
                                IsLocal = true
                            }).ShouldBlockClient);
                        });
        }

        [TestMethod]
        public void TestShouldBanUserAgentMethodWorks()
        {
            ServiceProvider.RunScopedService<IAntiDosFirewall>(antiDosFirewall =>
                        {
                            Assert.IsTrue(antiDosFirewall.ShouldBanUserAgent(new AntiDosFirewallRequestInfo
                            {
                                IP = "192.168.1.1",
                                UserAgent = "asafaweb",
                                UrlReferrer = new Uri("https://google.com"),
                                RawUrl = "http://localhost:5000/home",
                                IsLocal = true
                            }).ShouldBlockClient);
                        });
        }

        [TestMethod]
        public void TestShouldBlockClientMethodWorks()
        {
            ServiceProvider.RunScopedService<IAntiDosFirewall>(antiDosFirewall =>
                        {
                            Assert.IsTrue(antiDosFirewall.ShouldBlockClient(new AntiDosFirewallRequestInfo
                            (new HeaderDictionary(new Dictionary<string, StringValues>
                                {
                                    {"ACUNETIX",  "in use"}
                                }))
                            {
                                IP = "192.168.1.1",
                                UserAgent = "google",
                                UrlReferrer = new Uri("https://google.com"),
                                RawUrl = "http://localhost:5000/home",
                                IsLocal = true
                            }).ShouldBlockClient);
                        });
        }

        [TestMethod]
        public void TestShouldBanIpMethodWorks()
        {
            ServiceProvider.RunScopedService<IAntiDosFirewall>(antiDosFirewall =>
                        {
                            Assert.IsTrue(antiDosFirewall.ShouldBanIp(new AntiDosFirewallRequestInfo
                            {
                                IP = "192.168.1.1",
                                UserAgent = "google",
                                UrlReferrer = new Uri("https://google.com"),
                                RawUrl = "http://localhost:5000/home",
                                IsLocal = true
                            }).ShouldBlockClient);
                        });
        }
    }
}