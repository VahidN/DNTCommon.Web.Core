using System.Collections.Specialized;
using System.Web;

namespace DNTCommon.Web.Core;

/// <summary>
///     Allows adding params to the given URI.
/// </summary>
public class UriBuilderExtensions
{
    private readonly UriBuilder _builder;
    private readonly NameValueCollection _collection;

    /// <summary>
    ///     Allows adding params to the given URI.
    /// </summary>
    /// <param name="uri"></param>
    public UriBuilderExtensions(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        _builder = new UriBuilder(uri);
        _collection = HttpUtility.ParseQueryString(uri.Query);
    }

    /// <summary>
    ///     Allows adding params to the given URI.
    /// </summary>
    /// <param name="uri"></param>
    public UriBuilderExtensions(string uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        _builder = new UriBuilder(uri);
        _collection = HttpUtility.ParseQueryString(_builder.Uri.Query);
    }

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

    /// <summary>
    ///     Allows removing params to the given URI.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public UriBuilderExtensions RemoveParameter(string key)
    {
        _collection.Remove(key);

        return this;
    }

    /// <summary>
    ///     Allows adding or replacing params to the given URI.
    /// </summary>
    public UriBuilderExtensions AddOrUpdateParameter(string key, string value)
    {
        _collection.Set(key, value);

        return this;
    }

    /// <summary>
    ///     Allows replacing existing params to the given URI.
    /// </summary>
    public UriBuilderExtensions UpdateParameter(string key, string value)
    {
        if (_collection.AllKeys.Any(x => string.Equals(x, key, StringComparison.Ordinal)))
        {
            _collection.Set(key, value);
        }

        return this;
    }
}