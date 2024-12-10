﻿namespace DNTCommon.Web.Core;

/// <summary>
///     A high level utility that converts HTML to PDF.
/// </summary>
public interface IHtmlToPdfGenerator
{
    /// <summary>
    ///     High level method that converts HTML to PDF.
    ///     This method uses Google-Chrome to convert of the given URLs.
    ///     You can install this browser on Windows systems very easily.
    ///     On linux systems use these commands:
    ///     https://gist.github.com/Austcool-Walker/3f898ff04e5e0a9644ab487984e5eeec
    ///     wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | sudo apt-key add -
    ///     sudo sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >>
    ///     /etc/apt/sources.list.d/google.list'
    ///     sudo apt-get update
    ///     sudo apt-get install google-chrome-stable
    ///     google-chrome --version
    /// </summary>
    Task<string> GeneratePdfFromHtmlAsync(HtmlToPdfGeneratorOptions options);
}