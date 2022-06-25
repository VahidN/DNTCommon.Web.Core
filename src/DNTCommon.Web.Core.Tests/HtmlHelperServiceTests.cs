using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class HtmlHelperServiceTests : TestsBase
{
    public const string Html = @"
        <html>
        <head>
           <title>.NET Core Applications &#8211; DNT</title>
        </head>
        <body>
           <img width='300' id='X1' src='http://mysite.com/path/img1.png' />
           <a id='X2' HREF='http://mysite.com/path/img2.png'>image 2</a>
           <img id='X3' SRC='http://mysite.com/path/img2.png' >
           <img id='X4' src='http://mysite.com/path/img3.png?w=100&h=200' />
           <a id='X5' href='http://mysite.com/path/img1.png'>image 1</a>

           <img id='X6' src='/path/img1.png' />
           <img id='X7' src='file://path/img1.png' />
           <a id='X8' HREF='path/img2.png'>image 2</a>
           <img id='X9' SRC='./path/img2.png' >
           <img id='X10' src='../img3.png?w=100&h=200' />
           <a id='X11' href='www.site.com'>image 1</a>
           <img id='X12' width='300' src='data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjwhLS0gR2VuZXJhdG9yOiBBZG9iZSBJbGx1c3RyYXRvciAxOS4xLjAsIFNWRyBFeHBvcnQgUGx1Zy1JbiAuIFNWRyBWZXJzaW9uOiA2LjAwIEJ1aWxkIDApICAtLT4NCjxzdmcgdmVyc2lvbj0iMS4xIiBpZD0iTGF5ZXJfMSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4bWxuczp4bGluaz0iaHR0cDovL3d3dy53My5vcmcvMTk5OS94bGluayIgeD0iMHB4IiB5PSIwcHgiDQoJIHZpZXdCb3g9IjAgMCAyNTAgMjUwIiBzdHlsZT0iZW5hYmxlLWJhY2tncm91bmQ6bmV3IDAgMCAyNTAgMjUwOyIgeG1sOnNwYWNlPSJwcmVzZXJ2ZSI+DQo8c3R5bGUgdHlwZT0idGV4dC9jc3MiPg0KCS5zdDB7ZmlsbDojREQwMDMxO30NCgkuc3Qxe2ZpbGw6I0MzMDAyRjt9DQoJLnN0MntmaWxsOiNGRkZGRkY7fQ0KPC9zdHlsZT4NCjxnPg0KCTxwb2x5Z29uIGNsYXNzPSJzdDAiIHBvaW50cz0iMTI1LDMwIDEyNSwzMCAxMjUsMzAgMzEuOSw2My4yIDQ2LjEsMTg2LjMgMTI1LDIzMCAxMjUsMjMwIDEyNSwyMzAgMjAzLjksMTg2LjMgMjE4LjEsNjMuMiAJIi8+DQoJPHBvbHlnb24gY2xhc3M9InN0MSIgcG9pbnRzPSIxMjUsMzAgMTI1LDUyLjIgMTI1LDUyLjEgMTI1LDE1My40IDEyNSwxNTMuNCAxMjUsMjMwIDEyNSwyMzAgMjAzLjksMTg2LjMgMjE4LjEsNjMuMiAxMjUsMzAgCSIvPg0KCTxwYXRoIGNsYXNzPSJzdDIiIGQ9Ik0xMjUsNTIuMUw2Ni44LDE4Mi42aDBoMjEuN2gwbDExLjctMjkuMmg0OS40bDExLjcsMjkuMmgwaDIxLjdoMEwxMjUsNTIuMUwxMjUsNTIuMUwxMjUsNTIuMUwxMjUsNTIuMQ0KCQlMMTI1LDUyLjF6IE0xNDIsMTM1LjRIMTA4bDE3LTQwLjlMMTQyLDEzNS40eiIvPg0KPC9nPg0KPC9zdmc+DQo=' />
        </body>
        </html>
        ";

    [TestMethod]
    public void TestExtractImagesLinksReturnsAllImages()
    {
        ServiceProvider.RunScopedService<IHtmlHelperService>(htmlHelperService =>
        {
            var imagesList = htmlHelperService.ExtractImagesLinks(Html).ToList();
            Assert.AreEqual(expected: 8, actual: imagesList.Count);
        });
    }

    [TestMethod]
    public void TestExtractLinksReturnsAllLinks()
    {
        ServiceProvider.RunScopedService<IHtmlHelperService>(htmlHelperService =>
        {
            var linksList = htmlHelperService.ExtractLinks(Html).ToList();
            Assert.AreEqual(expected: 4, actual: linksList.Count);
        });
    }

    [TestMethod]
    public void TestGetHtmlPageTitleWorks()
    {
        ServiceProvider.RunScopedService<IHtmlHelperService>(htmlHelperService =>
        {
            var title = htmlHelperService.GetHtmlPageTitle(Html);
            Assert.AreEqual(expected: ".NET Core Applications – DNT", actual: title);
        });
    }

    [TestMethod]
    public void TestFixRelativeImagesOrUrlsWorks()
    {
        ServiceProvider.RunScopedService<IHtmlHelperService>(htmlHelperService =>
        {
            var html = htmlHelperService.FixRelativeUrls(Html, "content/path/image.png", "http://www.mysite.com");
            Assert.AreEqual(
                expected: "\n        <html>\n        <head>\n           <title>.NET Core Applications &#8211; DNT</title>\n        </head>\n        <body>\n           <img width='300' id='X1' src='http://mysite.com/path/img1.png'>\n           <a id='X2' href='http://mysite.com/path/img2.png'>image 2</a>\n           <img id='X3' src='http://mysite.com/path/img2.png'>\n           <img id='X4' src='http://mysite.com/path/img3.png?w=100&h=200'>\n           <a id='X5' href='http://mysite.com/path/img1.png'>image 1</a>\n\n           <img id='X6' src='http://www.mysite.com/path/img1.png'>\n           <img id='X7' src='http://www.mysite.com/content/path/image.png'>\n           <a id='X8' href='http://www.mysite.com/path/img2.png'>image 2</a>\n           <img id='X9' src='http://www.mysite.com/path/img2.png'>\n           <img id='X10' src='http://www.mysite.com/img3.png?w=100&h=200'>\n           <a id='X11' href='http://www.site.com'>image 1</a>\n           <img id='X12' width='300' src='data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjwhLS0gR2VuZXJhdG9yOiBBZG9iZSBJbGx1c3RyYXRvciAxOS4xLjAsIFNWRyBFeHBvcnQgUGx1Zy1JbiAuIFNWRyBWZXJzaW9uOiA2LjAwIEJ1aWxkIDApICAtLT4NCjxzdmcgdmVyc2lvbj0iMS4xIiBpZD0iTGF5ZXJfMSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB4bWxuczp4bGluaz0iaHR0cDovL3d3dy53My5vcmcvMTk5OS94bGluayIgeD0iMHB4IiB5PSIwcHgiDQoJIHZpZXdCb3g9IjAgMCAyNTAgMjUwIiBzdHlsZT0iZW5hYmxlLWJhY2tncm91bmQ6bmV3IDAgMCAyNTAgMjUwOyIgeG1sOnNwYWNlPSJwcmVzZXJ2ZSI+DQo8c3R5bGUgdHlwZT0idGV4dC9jc3MiPg0KCS5zdDB7ZmlsbDojREQwMDMxO30NCgkuc3Qxe2ZpbGw6I0MzMDAyRjt9DQoJLnN0MntmaWxsOiNGRkZGRkY7fQ0KPC9zdHlsZT4NCjxnPg0KCTxwb2x5Z29uIGNsYXNzPSJzdDAiIHBvaW50cz0iMTI1LDMwIDEyNSwzMCAxMjUsMzAgMzEuOSw2My4yIDQ2LjEsMTg2LjMgMTI1LDIzMCAxMjUsMjMwIDEyNSwyMzAgMjAzLjksMTg2LjMgMjE4LjEsNjMuMiAJIi8+DQoJPHBvbHlnb24gY2xhc3M9InN0MSIgcG9pbnRzPSIxMjUsMzAgMTI1LDUyLjIgMTI1LDUyLjEgMTI1LDE1My40IDEyNSwxNTMuNCAxMjUsMjMwIDEyNSwyMzAgMjAzLjksMTg2LjMgMjE4LjEsNjMuMiAxMjUsMzAgCSIvPg0KCTxwYXRoIGNsYXNzPSJzdDIiIGQ9Ik0xMjUsNTIuMUw2Ni44LDE4Mi42aDBoMjEuN2gwbDExLjctMjkuMmg0OS40bDExLjcsMjkuMmgwaDIxLjdoMEwxMjUsNTIuMUwxMjUsNTIuMUwxMjUsNTIuMUwxMjUsNTIuMQ0KCQlMMTI1LDUyLjF6IE0xNDIsMTM1LjRIMTA4bDE3LTQwLjlMMTQyLDEzNS40eiIvPg0KPC9nPg0KPC9zdmc+DQo='>\n        </body>\n        </html>\n        ",
                actual: html);
        });
    }
}