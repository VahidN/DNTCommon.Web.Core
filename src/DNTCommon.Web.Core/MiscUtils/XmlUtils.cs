using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DNTCommon.Web.Core;

/// <summary>
///     XML utils
/// </summary>
public static class XmlUtils
{
    private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(value: 1);

    /// <summary>
    ///     Pretty prints the given XML
    /// </summary>
    public static string FormatXml(this string xml, bool throwOnException)
    {
        try
        {
            var doc = XDocument.Parse(xml);

            return doc.ToString();
        }
        catch (Exception)
        {
            if (throwOnException)
            {
                throw;
            }

            return xml;
        }
    }

    /// <summary>
    ///     TextSyndicationContent's input sanitizer
    /// </summary>
    public static string SanitizeXmlString(this string? input)
    {
        if (input is null)
        {
            return "";
        }

        var sb = new StringBuilder(input.Length);
        Span<char> chars = stackalloc char[2];

        foreach (var rune in input.EnumerateRunes())
        {
            if (!rune.TryEncodeToUtf16(chars, out var written))
            {
                continue;
            }

            if (written == 1)
            {
                if (!XmlConvert.IsXmlChar(chars[index: 0]))
                {
                    continue;
                }
            }
            else if (written == 2)
            {
                if (!XmlConvert.IsXmlSurrogatePair(chars[index: 0], chars[index: 1]))
                {
                    continue;
                }
            }
            else
            {
                throw new InvalidOperationException(string.Create(CultureInfo.InvariantCulture,
                    $"written = {written}"));
            }

            sb.Append(chars[..written]);
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Does the given input contain XHTML?
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsXHTML(this string? input)
    {
        try
        {
            if (input.IsEmpty())
            {
                return false;
            }

            var x = XElement.Parse($"<wrapper>{input}</wrapper>");

            return !(x.DescendantNodes().Count() == 1 && x.DescendantNodes().First().NodeType == XmlNodeType.Text);
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    ///     Does the given input contain HTML?
    /// </summary>
    public static bool ContainsHtmlTag(this string? text, string tagName)
        => !text.IsEmpty() && Regex.IsMatch(text, $@"<\s*{tagName}\s*\/?>", RegexOptions.IgnoreCase, OneMinute);

    /// <summary>
    ///     Does the given input contain HTML?
    /// </summary>
    public static bool ContainsHtmlTags(this string? text, string tagNames)
    {
        ArgumentNullException.ThrowIfNull(tagNames);

        return !text.IsEmpty() && tagNames.Split(separator: '|').Any(text.ContainsHtmlTag);
    }

    /// <summary>
    ///     Does the given input contain HTML?
    /// </summary>
    public static bool ContainsHtmlTags(this string? text)
        => text.ContainsHtmlTags(
            tagNames:
            "a|abbr|acronym|address|area|b|base|bdo|big|blockquote|body|br|button|caption|cite|code|col|colgroup|dd|del|dfn|div|dl|DOCTYPE|dt|em|fieldset|form|h1|h2|h3|h4|h5|h6|head|html|hr|i|img|input|ins|kbd|label|legend|li|link|map|meta|noscript|object|ol|optgroup|option|p|param|pre|q|samp|script|select|small|span|strong|style|sub|sup|table|tbody|td|textarea|tfoot|th|thead|title|tr|tt|ul|var");

    /// <summary>
    ///     Converts an XmlDocument to a string
    /// </summary>
    public static string? ToXmlString([NotNullIfNotNull(nameof(document))] this XmlDocument? document)
    {
        if (document is null)
        {
            return null;
        }

        using var ms = new MemoryStream();

        var settings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8
        };

        using var xmlWriter = XmlWriter.Create(ms, settings);
        document.Save(xmlWriter);
        ms.Position = 0;
        using var streamReader = new StreamReader(ms);

        return streamReader.ReadToEnd();
    }

    /// <summary>
    ///     Uses XmlSerializer to serialize the given object into an XML document
    /// </summary>
    public static XmlDocument? ToXmlDocument<T>([NotNullIfNotNull(nameof(value))] this T? value)
        where T : class
    {
        if (value is null)
        {
            return null;
        }

        var serializer = new XmlSerializer(value.GetType());
        var sb = new StringBuilder();
        using var writer = new StringWriter(sb);
        var ns = new XmlSerializerNamespaces();
        ns.Add(prefix: "", ns: "");
        serializer.Serialize(writer, value, ns);
        var doc = new XmlDocument();
        doc.LoadXml(sb.ToString());

        return doc;
    }

    /// <summary>
    ///     Uses XmlSerializer to serialize the given object into an XML document with a digital signature.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="rsaKeys">An RSA key information from an XML string.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string? ToSignedXmlDocumentString<T>([NotNullIfNotNull(nameof(value))] this T? value, RsaKeys rsaKeys)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(rsaKeys);

        if (rsaKeys.PrivateKey is not null)
        {
            return ToSignedXmlDocumentString(value, rsaKeys.PrivateKey);
        }

        if (rsaKeys.PrivateKeyXml is not null)
        {
            return ToSignedXmlDocumentString(value, rsaKeys.PrivateKeyXml);
        }

        return null;
    }

    /// <summary>
    ///     Uses XmlSerializer to serialize the given object into an XML document with a digital signature.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="rsaPrivateKeyXml">An RSA key information from an XML string.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string? ToSignedXmlDocumentString<T>([NotNullIfNotNull(nameof(value))] this T? value,
        string rsaPrivateKeyXml)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(rsaPrivateKeyXml);

        if (value is null)
        {
            return null;
        }

        using var rsa = RSA.Create();
        rsa.FromXmlString(rsaPrivateKeyXml);

        return ToSignedXmlDocumentString(value, rsa);
    }

    /// <summary>
    ///     Uses XmlSerializer to serialize the given object into an XML document with a digital signature.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="rsa">An RSA key information with private key.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string? ToSignedXmlDocumentString<T>([NotNullIfNotNull(nameof(value))] this T? value, RSA rsa)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(rsa);

        if (value is null)
        {
            return null;
        }

        var xmlDocument = value.ToXmlDocument();

        if (xmlDocument is null)
        {
            return null;
        }

        var xmlDigitalSignature = GetXmlDigitalSignature(xmlDocument, rsa);
        AppendDigitalSignature(xmlDocument, xmlDigitalSignature);

        return xmlDocument.ToXmlString();
    }

    /// <summary>
    ///     Uses XmlSerializer to Deserialize the given signed XML into an object.
    /// </summary>
    /// <param name="signedXml"></param>
    /// <param name="rsaKeys">An RSA key information from an XML string.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static OperationResult<T?> DeserializeSignedXmlDocumentString<T>(this string? signedXml, RsaKeys rsaKeys)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(rsaKeys);

        if (rsaKeys.PublicKey is not null)
        {
            return DeserializeSignedXmlDocumentString<T>(signedXml, rsaKeys.PublicKey);
        }

        if (rsaKeys.PublicKeyXml is not null)
        {
            return DeserializeSignedXmlDocumentString<T>(signedXml, rsaKeys.PublicKeyXml);
        }

        return ("PublicKey is null", OperationStat.Failed, null);
    }

    /// <summary>
    ///     Uses XmlSerializer to Deserialize the given signed XML into an object.
    /// </summary>
    /// <param name="signedXml"></param>
    /// <param name="rsaPublicKeyXml">An RSA key information from an XML string.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static OperationResult<T?> DeserializeSignedXmlDocumentString<T>(this string? signedXml,
        string rsaPublicKeyXml)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(rsaPublicKeyXml);

        if (signedXml is null)
        {
            return ("Input data is null.", OperationStat.Failed, null);
        }

        using var rsa = RSA.Create();
        rsa.FromXmlString(rsaPublicKeyXml);

        return DeserializeSignedXmlDocumentString<T>(signedXml, rsa);
    }

    /// <summary>
    ///     Uses XmlSerializer to Deserialize the given signed XML into an object.
    /// </summary>
    /// <param name="signedXml"></param>
    /// <param name="rsa">An RSA key information with public key.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static OperationResult<T?> DeserializeSignedXmlDocumentString<T>(this string? signedXml, RSA rsa)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(rsa);

        if (signedXml is null)
        {
            return ("Input data is null.", OperationStat.Failed, null);
        }

        var doc = new XmlDocument();
        doc.LoadXml(signedXml);

        var xmlNamespaceManager = new XmlNamespaceManager(doc.NameTable);
        xmlNamespaceManager.AddNamespace(prefix: "sig", uri: "http://www.w3.org/2000/09/xmldsig#");

        var xml = new SignedXml(doc);

        if (doc.SelectSingleNode(xpath: "//sig:Signature", xmlNamespaceManager) is not XmlElement signatureNode)
        {
            return ("This license file is not signed.", OperationStat.Failed, null);
        }

        xml.LoadXml(signatureNode);

        if (!xml.CheckSignature(rsa))
        {
            return ("This license file is not valid.", OperationStat.Failed, null);
        }

        var ourXml = xml.GetXml();

        if (ourXml.OwnerDocument.DocumentElement is null)
        {
            return ("This license file is corrupted.", OperationStat.Failed, null);
        }

        using var reader = new XmlNodeReader(ourXml.OwnerDocument.DocumentElement);

        var xmlSerializer = new XmlSerializer(typeof(T));

        return (T?)Convert.ChangeType(xmlSerializer.Deserialize(reader), typeof(T), CultureInfo.InvariantCulture);
    }

    private static XmlElement GetXmlDigitalSignature(XmlDocument xmlDocument, AsymmetricAlgorithm key)
    {
        var xml = new SignedXml(xmlDocument)
        {
            SigningKey = key
        };

        var reference = new Reference
        {
            Uri = ""
        };

        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        xml.AddReference(reference);
        xml.ComputeSignature();

        return xml.GetXml();
    }

    private static void AppendDigitalSignature(XmlDocument xmlDocument, XmlNode xmlDigitalSignature)
        => xmlDocument.DocumentElement?.AppendChild(xmlDocument.ImportNode(xmlDigitalSignature, deep: true));
}
