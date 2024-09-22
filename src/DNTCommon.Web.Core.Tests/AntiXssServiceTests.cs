using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNTCommon.Web.Core.Tests;

[TestClass]
public class AntiXssServiceTests : TestsBase
{
    [TestMethod]
    public void AntiXssServiceShouldNotChangeLink()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<a href='http://site.com'>site</a>");
            var expected = "<a href='http://site.com'>site</a>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceShouldRemoveUndefinedAttribute()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<a attr='val1' href='http://site.com'>site</a>");
            var expected = "<a href='http://site.com'>site</a>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceShouldRemoveUndefinedTag()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<a href='http://site.com'>site</a><tag1>text</tag1>");
            var expected = "<a href='http://site.com'>site</a>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest1()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<a href=\"'';!--\"<XSS>=&{()}\">");
            var expected = "<a href=\"'';!--\"></a>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest2()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=\"javascript:alert('XSS');\">");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest3()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=javascript:alert('XSS')>");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest4()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=JaVaScRiPt:alert('XSS')>");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest5()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=javascript:alert(&quot;XSS&quot;)>");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest6()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=`javascript:alert(\"RSnake says, 'XSS'\")`>");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest7()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG \"\"\"><SCRIPT>alert(\"XSS\")</SCRIPT>\">");
            var expected = "<img>\">";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest8()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual =
                antiXssService.GetSanitizedHtml(html: "<IMG SRC=javascript:alert(String.fromCharCode(88,83,83))>");

            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest9()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                "<IMG SRC=&#106;&#97;&#118;&#97;&#115;&#99;&#114;&#105;&#112;&#116;&#58;&#97;&#108;&#101;&#114;&#116;&#40;&#39;&#88;&#83;&#83;&#39;&#41;>");

            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest10()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                "<IMG SRC=&#0000106&#0000097&#0000118&#0000097&#0000115&#0000099&#0000114&#0000105&#0000112&#0000116&#0000058&#0000097&#0000108&#0000101&#0000114&#0000116&#0000040&#0000039&#0000088&#0000083&#0000083&#0000039&#0000041>");

            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest11()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                "<IMG SRC=&#x6A&#x61&#x76&#x61&#x73&#x63&#x72&#x69&#x70&#x74&#x3A&#x61&#x6C&#x65&#x72&#x74&#x28&#x27&#x58&#x53&#x53&#x27&#x29>");

            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest12()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=\"jav	ascript:alert('XSS');\">");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest13()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=\"jav&#x09;ascript:alert('XSS');\">");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest14()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=\"jav&#x0A;ascript:alert('XSS');\">");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest15()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=\"jav&#x0D;ascript:alert('XSS');\">");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest16()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: @"<IMG
SRC
=
""
j
a
v
a
s
c
r
i
p
t
:
a
l
e
r
t
(
'
X
S
S
'
)
""
>
");

            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest17()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=java\0script:alert(\"XSS\")>");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest18()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<SCR\0IPT>alert(\"XSS\")</SCR\0IPT>");
            var expected = "";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest19()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=\" &#14;  javascript:alert('XSS');\">");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest20()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=\"javascript:alert('XSS')\"");
            var expected = "";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest21()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<image src=http://ha.ckers.org/scriptlet.html <");
            var expected = "";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest22()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<div style=\"\";alert('XSS');//\">");
            var expected = "<div></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest23()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual =
                antiXssService.GetSanitizedHtml(html: "<INPUT TYPE=\"IMAGE\" SRC=\"javascript:alert('XSS');\">");

            var expected = "<input type=\"IMAGE\">";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest24()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<BR SIZE=\"&{alert('XSS')}\">");
            var expected = "<br>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest25()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC='vbscript:msgbox(\"XSS\")'>");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest26()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=\"mocha:[code]\">");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest27()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG SRC=\"Livescript:[code]\">");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest28()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IFRAME SRC=\"javascript:alert('XSS');\"></IFRAME>");
            var expected = "<iframe></iframe>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest29()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual =
                antiXssService.GetSanitizedHtml(html: "<FRAMESET><FRAME SRC=\"javascript:alert('XSS');\"></FRAMESET>");

            var expected = "";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest30()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<TABLE BACKGROUND=\"javascript:alert('XSS')\">");
            var expected = "<table></table>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest31()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<TABLE><TD BACKGROUND=\"javascript:alert('XSS')\">");
            var expected = "<table><td></td></table>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest32()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual =
                antiXssService.GetSanitizedHtml(html: "<DIV STYLE=\"background-image: url(javascript:alert('XSS'))\">");

            var expected = "<div></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest33()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                @"<DIV STYLE=""background-image:\0075\0072\006C\0028'\006a\0061\0076\0061\0073\0063\0072\0069\0070\0074\003a\0061\006c\0065\0072\0074\0028\0027\0058\0053\0053\0027\0029'\0029"">");

            var expected = "<div></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest34()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<DIV STYLE=\"width: expression(alert('XSS'));\">");
            var expected = "<div></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest35()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<DIV STYLE=\"width: expression(alert('XSS'));\">");
            var expected = "<div></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest36()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<IMG STYLE=\"xss:expr/*XSS*/ession(alert('XSS'))\">");
            var expected = "<img>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest37()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                "<HTML><BODY><?xml:namespace prefix=\"t\" ns=\"urn:schemas-microsoft-com:time\"><?import namespace=\"t\" implementation=\"#default#time2\"><t:set attributeName=\"innerHTML\" to=\"XSS&lt;SCRIPT DEFER&gt;alert(&quot;XSS&quot;)&lt;/SCRIPT&gt;\"></BODY></HTML>");

            var expected =
                "<html><body><?xml:namespace prefix=\"t\" ns=\"urn:schemas-microsoft-com:time\"><?import namespace=\"t\" implementation=\"#default#time2\"></body></html>";

            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest38()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual =
                antiXssService.GetSanitizedHtml(
                    html: "<A HREF=\"javascript:document.location='http://www.google.com/'\">XSS</A>");

            var expected = "<a>XSS</a>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest39()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                "<Div style=\"background-color: http://www.codeplex.com?url=<SCRIPT SRC=http://ha.ckers.org/xss.js></SCRIPT>\">");

            var expected = "<div></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest40()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html: "<Div style=\"background-color: expression(<SCRIPT SRC=http://ha.ckers.org/xss.js></SCRIPT>)\">");

            var expected = "<div></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest41()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                "<Div style=\"background-color: http://www.codeplex.com?url=<<SCRIPT>alert(\"XSS\");//<</SCRIPT>\">");

            var expected = "<div></div>\">";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest42()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual =
                antiXssService.GetSanitizedHtml(
                    html: "<A HREF=\"http://www.codeplex.com?url=¼script¾alert(¢XSS¢)¼/script¾\">XSS</A>");

            var expected = "<a>XSS</a>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest43()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                "<A HREF=\"http://www.codeplex.com?url=<!--[if gte IE 4]><SCRIPT>alert('XSS');</SCRIPT><![endif]-->\">XSS</A>");

            var expected = "<a>XSS</a>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest44()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                "<div style=\"background-color: test;\">Test<img src=\"http://www.example.com/test.gif\" style=\"background-image: url(http://www.example.com/bg.jpg); margin: 10px\"></div>");

            var expected =
                "<div style=\"background-color: test;\">Test<img src=\"http://www.example.com/test.gif\" style=\"background-image: url(http://www.example.com/bg.jpg); margin: 10px\"></div>";

            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest45()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: "<div><!-- comments --></div>");
            var expected = "<div></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest46()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual =
                antiXssService.GetSanitizedHtml(html: @"<div style=""top:exp\72 ess\000069 on(alert())"">XSS</div>");

            var expected = "<div>XSS</div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest47()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: @"<div style=""top:ｅｘｐｒｅｓｓｉｏｎ(alert())"">XSS</div>");
            var expected = "<div>XSS</div>";
            Assert.AreEqual(expected, actual);

            actual = antiXssService.GetSanitizedHtml(html: @"<div style=""top:ＥＸＰＲＥＳＳＩＯＮ(alert())"">XSS</div>");
            expected = "<div>XSS</div>";
            Assert.AreEqual(expected, actual);

            actual = antiXssService.GetSanitizedHtml(html: @"<div style=""top:expʀessɪoɴ(alert())"">XSS</div>");
            expected = "<div>XSS</div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest48()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: @"<div data-test1=""value x""></div>",
                allowDataAttributes: true);

            var expected = @"<div data-test1=""value x""></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest49()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: @"<div data-test1=""value x""></div>",
                allowDataAttributes: false);

            var expected = @"<div></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest50()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual =
                antiXssService.GetSanitizedHtml(
                    html: "<html><head><title>Test</title></head><body><div>Test</div></body></html>");

            var expected = "<html><head><title>Test</title></head><body><div>Test</div></body></html>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest51()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: @"
                <div>
<p>
This is my text
</p>

<p>

<span>text  text</span> This is next text

</p>

</span>
");

            var expected = "<div><p>\nThis is my text\n</p><p><span>text  text</span> This is next text\n\n</p></div>";
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest52()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(html: @"
<code>
Line1
Line2
</code>
");

            var expected =
                @"<code dir=""ltr"" style=""white-space: pre-wrap; overflow: auto; word-break: break-word; text-align: left;"">
Line1
Line2
</code>";

            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest53()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var html =
                @"<pre language=""CSharp"" name=""code"" dir=""ltr"" style=""white-space: pre-wrap; overflow: auto; word-break: break-word; text-align: left;"">using (BulkInsertOperation bulkInsert = store.BulkInsert())
{
    for (int i = 0; i &lt; 1000 * 1000; i++)
    {
        bulkInsert.Store(new User
        {
            PhoneNumber = randomPhone(),
        });
    }
}</pre>";

            var actual = antiXssService.GetSanitizedHtml(html);
            var expected = html;
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest54()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var html = @"<pre language=""CSharp"" name=""code"">
<script>
  alert('XSS');
</script>
</pre>";

            var actual = antiXssService.GetSanitizedHtml(html);

            var expected =
                "<pre language=\"CSharp\" name=\"code\" dir=\"ltr\" style=\"white-space: pre-wrap; overflow: auto; word-break: break-word; text-align: left;\">\n&lt;script&gt;\n  alert(&#39;XSS&#39;);\n&lt;/script&gt;\n</pre>";

            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTest55()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var html =
                "<pre language=\"CSharp\" name=\"code\" dir=\"ltr\" style=\"white-space: pre-wrap; overflow: auto; word-break: break-word; text-align: left;\">\n&lt;script&gt;\n  alert(&#39;XSS&#39;);\n&lt;/script&gt;\n</pre>";

            var actual = antiXssService.GetSanitizedHtml(html);
            var expected = html;
            Assert.AreEqual(expected, actual);
        });

    [TestMethod]
    public void AntiXssServiceTestPToBrWorks()
        => ServiceProvider.RunScopedService<IAntiXssService>(antiXssService =>
        {
            var actual = antiXssService.GetSanitizedHtml(
                html:
                "<p>this is a test1</p> <p>this is a test2</p><div>this is a test3<div>test4</div></div><p class=\"ql-direction-rtl ql-align-right\"><br></p><div class=\"ql-direction-rtl ql-align-right\"></div>",
                htmlModificationRules: new HtmlModificationRules
                {
                    ConvertPToDiv = true
                });

            var expected =
                "<div>this is a test1</div><div>this is a test2</div><div>this is a test3<div>test4</div></div><div class=\"ql-direction-rtl ql-align-right\"><br></div><div class=\"ql-direction-rtl ql-align-right\"><br></div>";

            Assert.AreEqual(expected, actual);
        });
}