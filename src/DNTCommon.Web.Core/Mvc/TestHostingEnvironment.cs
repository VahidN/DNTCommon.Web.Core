using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.FileProviders;

namespace DNTCommon.Web.Core;

/// <summary>
/// Provides information about the web hosting environment an application is running in.
/// </summary>
public class TestHostingEnvironment : IWebHostEnvironment
{
    /// <summary>
    /// Provides information about the web hosting environment an application is running in.
    /// </summary>
    public TestHostingEnvironment()
    {
        ApplicationName = typeof(TestHostingEnvironment).Assembly.FullName ?? nameof(TestHostingEnvironment);
        WebRootFileProvider = new NullFileProvider();
        ContentRootFileProvider = new NullFileProvider();
        ContentRootPath = AppContext.BaseDirectory;
        WebRootPath = AppContext.BaseDirectory;
    }

    /// <summary>
    /// Gets or sets the name of the environment. The host automatically sets this property
    /// to the value of the of the "environment" key as specified in configuration.
    /// </summary>
    public string EnvironmentName { get; set; } = nameof(TestHostingEnvironment);

    /// <summary>
    /// Gets or sets the name of the application. This property is automatically set
    /// by the host to the assembly containing the application entry point.
    /// </summary>
    public string ApplicationName { get; set; }

    /// <summary>
    /// Gets or sets the absolute path to the directory that contains the web-servable application content files.
    /// </summary>
    public string WebRootPath { get; set; }

    /// <summary>
    /// Gets or sets an Microsoft.Extensions.FileProviders.IFileProvider pointing at
    /// Microsoft.AspNetCore.Hosting.IWebHostEnvironment.WebRootPath.
    /// </summary>
    public IFileProvider WebRootFileProvider { get; set; }

    /// <summary>
    /// Gets or sets the absolute path to the directory that contains the application
    /// content files.
    /// </summary>
    public string ContentRootPath { get; set; }

    /// <summary>
    /// Gets or sets an Microsoft.Extensions.FileProviders.IFileProvider pointing at
    /// Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath.
    /// </summary>
    public IFileProvider ContentRootFileProvider { get; set; }
}