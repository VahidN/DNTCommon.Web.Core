using System.Collections.Specialized;
using System.Web;

namespace DNTCommon.Web.Core;

/// <summary>
///     Allows adding params to the given URI.
/// </summary>
/// <param name="uri"></param>
public class UriBuilderExtensions(Uri uri)
{
    private readonly UriBuilder _builder = new(uri);
    private readonly NameValueCollection _collection = HttpUtility.ParseQueryString(string.Empty);

    /// <summary>
    ///     The final URL
    /// </summary>
    public Uri Uri
    {
        get
        {
            _builder.Query = _collection.ToString();

            return _builder.Uri;
        }
    }

    /// <summary>
    ///     The final URL
    /// </summary>
    public string RelativeUrl
    {
        get
        {
            var finalUri = Uri;

            return finalUri.IsAbsoluteUri ? finalUri.PathAndQuery : finalUri.ToString();
        }
    }

    /// <summary>
    ///     Allows adding params to the given URI.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public UriBuilderExtensions AddParameter(string key, string value)
    {
        _collection.Add(key, value);

        return this;
    }
}