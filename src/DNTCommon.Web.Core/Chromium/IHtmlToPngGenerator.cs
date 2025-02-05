namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PNG.
/// </summary>
public interface IHtmlToPngGenerator
{
    /// <summary>
    ///     High level method that converts HTML to PNG.
    ///     This method uses Google-Chrome to take screenshots of the given URLs.
    ///     You can install this browser on Windows systems very easily.
    ///     On linux systems use these commands:
    ///     https://gist.github.com/Austcool-Walker/3f898ff04e5e0a9644ab487984e5eeec
    ///     wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | sudo apt-key add -
    ///     sudo sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >>
    ///     /etc/apt/sources.list.d/google.list'
    ///     sudo apt-get update
    ///     sudo apt-get install google-chrome-stable
    ///     google-chrome --version
    ///     Also you should run this command to allow Google-Chrome create its own temp files:
    ///     <![CDATA[ CHROME_DIRS="/var/www/.local /var/www/.config /var/www/.cache /var/www/.pki" && mkdir -p ${CHROME_DIRS} && chown www-data ${CHROME_DIRS} ]]>
    /// </summary>
    Task<string> GeneratePngFromHtmlAsync(HtmlToPngGeneratorOptions options);
}