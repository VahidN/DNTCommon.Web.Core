using System.Text;

namespace DNTCommon.Web.Core;

/// <summary>
///     HttpResponseMessage Extensions
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    ///     Includes the response's body in the final error message.
    /// </summary>
    public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = $"StatusCode: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}";

        throw new SimpleHttpResponseException(response.StatusCode, content);
    }

    /// <summary>
    ///     Includes the response's body in the final error message.
    /// </summary>
    public static void EnsureSuccessStatusCode(this HttpResponseMessage response, Encoding? encoding = null)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        using var reader = new StreamReader(response.Content.ReadAsStream(), encoding ?? Encoding.UTF8);
        var responseContent = reader.ReadToEnd();

        var content = $"StatusCode: {response.StatusCode}, {responseContent}";

        throw new SimpleHttpResponseException(response.StatusCode, content);
    }
}
