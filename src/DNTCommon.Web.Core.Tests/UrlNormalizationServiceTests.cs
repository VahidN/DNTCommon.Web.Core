using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests
{
    [TestClass]
    public class UrlNormalizationServiceTests : TestsBase
    {
        [TestMethod]
        public async Task Test1ConvertingTheSchemeAndHostToLowercase()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
            {
                var url1 = await urlNormalizationService.NormalizeUrlAsync("HTTP://www.Example.com/", findRedirectUrl: false);
                var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/", findRedirectUrl: false);

                Assert.AreEqual(url1, url2);
            });
        }

        [TestMethod]
        public async Task Test2CapitalizingLettersInEscapeSequences()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/a%c2%b1b", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/a%C2%B1b", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test3DecodingPercentEncodedOctetsOfUnreservedCharacters()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/%7Eusername/", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/~username/", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test4RemovingTheDefaultPort()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com:80/bar.html", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/bar.html", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test5AddingTrailing()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/alice", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/alice/?", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test6RemovingDotSegments()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/../a/b/../c/./d.html", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/a/c/d.html", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test7RemovingDirectoryIndex1()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/default.asp", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test7RemovingDirectoryIndex2()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/default.asp?id=1", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/default.asp?id=1", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test7RemovingDirectoryIndex3()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/a/index.html", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/a/", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test8RemovingTheFragment()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/bar.html#section1", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/bar.html", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test9LimitingProtocols()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("https://www.example.com/", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test10RemovingDuplicateSlashes()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/foo//bar.html", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com/foo/bar.html", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test11AddWww()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://example.com/", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://www.example.com", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test11AddWww2()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://bob.example.com/", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://bob.example.com", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test12RemoveFeedburnerPart()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("http://site.net/2013/02/firefox-19-released/?utm_source=rss&utm_medium=rss&utm_campaign=firefox-19-released", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("http://site.net/2013/02/firefox-19-released", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }

        [TestMethod]
        public async Task Test13QueryStrings()
        {
            await ServiceProvider.RunScopedServiceAsync<IUrlNormalizationService>(async urlNormalizationService =>
           {
               var url1 = await urlNormalizationService.NormalizeUrlAsync("https://www.youtube.com/watch?v=3TcduzoIA64", findRedirectUrl: false);
               var url2 = await urlNormalizationService.NormalizeUrlAsync("https://www.youtube.com/watch?v=3TcduzoIA64", findRedirectUrl: false);

               Assert.AreEqual(url1, url2);
           });
        }
    }
}