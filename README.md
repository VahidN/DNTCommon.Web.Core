# DNTCommon.Web.Core

<p align="left">
  <a href="https://github.com/VahidN/DNTCommon.Web.Core">
     <img alt="GitHub Actions status" src="https://github.com/VahidN/DNTCommon.Web.Core/workflows/.NET%20Core%20Build/badge.svg">
  </a>
</p>

`DNTCommon.Web.Core` provides common scenarios' solutions for ASP.NET Core applications.

## Install via NuGet

To install DNTCommon.Web.Core, run the following command in the Package Manager Console:

```powershell
PM> Install-Package DNTCommon.Web.Core
```

You can also view the [package page](http://www.nuget.org/packages/DNTCommon.Web.Core/) on NuGet.


## Linux (and containers) support

The `SkiaSharp` library needs extra dependencies to work on Linux and containers. Please install the following NuGet packages:

```
PM> Install-Package SkiaSharp.NativeAssets.Linux.NoDependencies
PM> Install-Package HarfBuzzSharp.NativeAssets.Linux
```

You also need to modify your `.csproj` file to include some MSBuild directives that ensure the required files are in a good place. These extra steps are normally not required but seems to be some issues on how .NET loads them.

```xml
<Target Name="CopyFilesAfterPublish" AfterTargets="AfterPublish">
    <Copy SourceFiles="$(TargetDir)runtimes/linux-x64/native/libSkiaSharp.so" DestinationFolder="$([System.IO.Path]::GetFullPath('$(PublishDir)'))/bin/" />
    <Copy SourceFiles="$(TargetDir)runtimes/linux-x64/native/libHarfBuzzSharp.so" DestinationFolder="$([System.IO.Path]::GetFullPath('$(PublishDir)'))/bin/" />    
</Target>
```


## Usage

After installing the DNTCommon.Web.Core package, to register its default providers, call `services.AddDNTCommonWeb();` method in your [Startup class](/src/DNTCommon.Web.Core.TestWebApp/Startup.cs).

```csharp
using DNTCommon.Web.Core;

namespace MyWebApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDNTCommonWeb();
        }
```

## Features

- ActionResults

  - [FeedResult](/src/DNTCommon.Web.Core.TestWebApp/Controllers/FeedResultController.cs) is an ASP.NET Core RSS feed renderer.
  - [OpenSearchResult](/src/DNTCommon.Web.Core.TestWebApp/Controllers/OpenSearchController.cs) is an ASP.NET Core OpenSearch description format provider.
  - [SitemapResult](/src/DNTCommon.Web.Core.TestWebApp/Controllers/SitemapResultController.cs) is an ASP.NET Core Sitemap renderer.

- Caching

  - [ICacheService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/CacheServiceController.cs) encapsulates IMemoryCache functionalities.
  - [[NoBrowserCache]](/src/DNTCommon.Web.Core.TestWebApp/Controllers/CacheManagerExtentionsController.cs) action filter sets `no-cache`, `must-revalidate`, `no-store` headers for the current `Response`.

- DependencyInjection

  - [IServiceProviderExtensions](/src/DNTCommon.Web.Core/DependencyInjection/IServiceProviderExtensions.cs) creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.

- Drawing

  - [TextToImageExtensions](/src/DNTCommon.Web.Core.TestWebApp/Controllers/TextToImageController.cs) and [TextToImageResult](/src/DNTCommon.Web.Core.TestWebApp/Controllers/TextToImageController.cs) draw a text on a bitmap and then return it as a png byte array.

- Http

  - [ICommonHttpClientFactory](/src/DNTCommon.Web.Core.TestWebApp/Controllers/CommonHttpClientFactoryController.cs) service, reuses a single HttpClient instance across a multi-threaded application.
  - [DomainHelperExtensions](/src/DNTCommon.Web.Core.TestWebApp/Views/DomainHelperExtensions/Index.cshtml) provides useful extension methods to extract domain info of the URL's.
  - [IDownloaderService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/DownloaderServiceController.cs) encapsulates HttpClient's best practices of downloading large files.
  - [IHtmlHelperService](/src/DNTCommon.Web.Core.Tests/HtmlHelperServiceTests.cs) provides helper methods to work with HTML contents such as extracting links and titles or changing relative Urls to absolute Urls.
  - [IHttpRequestInfoService](/src/DNTCommon.Web.Core.TestWebApp/Views/HttpRequestInfo/Index.cshtml) provides useful methods to work with current HttpContext.Request.
  - [IRedirectUrlFinderService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/RedirectUrlFinderServiceController.cs) finds the actual hidden URL after multiple redirects.
  - [IUrlNormalizationService](/src/DNTCommon.Web.Core.Tests/UrlNormalizationServiceTests.cs) transforms a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
  - [IHtmlReaderService](/src/DNTCommon.Web.Core.Tests/HtmlReaderServiceTests.cs) reads the HTML document's nodes recursively.

- Mail

  - [IWebMailService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/WebMailServiceController.cs) simplifies sending an email using the `MailKit` library. It's able to use razor based email templates.

- ModelBinders

  - [PersianDateModelBinderProvider](/src/DNTCommon.Web.Core.TestWebApp/Views/PersianDateModelBinder/Index.cshtml) parses an incoming Persian date and then binds it to a DateTime property automatically. To use it globally (assuming your app only sends Persian dates to the server), Add it to MvcOptions: `services.AddMvc(options => options.UsePersianDateModelBinder())` or just apply it to an specific view-model `[ModelBinder(BinderType = typeof(PersianDateModelBinder))]`.
  - [YeKeModelBinderProvider](/src/DNTCommon.Web.Core.TestWebApp/Views/YeKeModelBinder/Index.cshtml) parses an incoming text and then corrects its Ye & Ke characters automatically. To use it globally, Add it to MvcOptions: `services.AddMvc(options => options.UseYeKeModelBinder())` or just apply it to an specific view-model `[ModelBinder(BinderType = typeof(YeKeModelBinder))]`.
  - [ApplyCorrectYeKeFilterAttribute](/src/DNTCommon.Web.Core.TestWebApp/Views/YeKeModelBinder/Index.cshtml) parses an incoming text and then corrects its Ye & Ke characters automatically. To use it globally, Add it to MvcOptions: `services.AddControllersWithViews(options => options.Filters.Add(typeof(ApplyCorrectYeKeFilterAttribute)))`.


- Mvc

  - [ControllerExtensions](/src/DNTCommon.Web.Core.TestWebApp/Controllers/ControllerExtensionsController.cs) provides useful extension methods to work with MVC Controllers.
  - [IMvcActionsDiscoveryService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/MvcActionsDiscoveryController.cs) provides a way to list all of the controllers and action methods of an MVC application.
  - [IViewRendererService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/ViewRendererServiceController.cs) helps rendering a .cshtml file as an string. It's useful for creating razor based email templates.
  - [UploadFileService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/AllowUploadSafeFilesController.cs) saves the posted IFormFile to the specified directory asynchronously.

- Schedulers

  - [BackgroundQueueController](/src/DNTCommon.Web.Core.TestWebApp/Controllers/BackgroundQueueController.cs) A .NET Core replacement for the old `HostingEnvironment.QueueBackgroundWorkItem` method.
  - [ScheduledTasksController](/src/DNTCommon.Web.Core.TestWebApp/Controllers/ScheduledTasksController.cs) DNTScheduler.Core is a lightweight ASP.NET Core's background tasks runner and scheduler. To define a new task, [create a new class](/src/DNTCommon.Web.Core.TestWebApp/ScheduledTasks/) that implements the `IScheduledTask` interface. To register this new task, call `services.AddDNTScheduler();` method in your [Startup class](/src/DNTCommon.Web.Core.TestWebApp/Startup.cs). `AddDNTScheduler` method, adds this new task to the list of the defined tasks. Also its first parameter defines the custom logic of the running intervals of this task. It's a callback method that will be called every second and provides the utcNow value. If it returns true, the job will be executed.If you have multiple jobs at the same time, the `order` parameter's value indicates the order of their execution.

- Security
  - [[AjaxOnly]](/src/DNTCommon.Web.Core.TestWebApp/Controllers/AjaxExtensionsController.cs) action filter determines whether the HttpRequest's `X-Requested-With` header has `XMLHttpRequest` value.
  - [IProtectionProviderService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/ProtectionProviderServiceController.cs) is an encryption provider based on `Microsoft.AspNetCore.DataProtection.IDataProtector`. It's only useful for short-term encryption scenarios such as creating encrypted HTTP cookies.
  - [IFileNameSanitizerService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/FileNameSanitizerServiceController.cs) determines whether the requested file is safe to download to avoid `Directory Traversal` & `File Inclusion` attacks.
  - [UploadFileExtensions](/src/DNTCommon.Web.Core.TestWebApp/Models/UserFileViewModel.cs) attribute determines only selected file extensions are safe [to be uploaded](/src/DNTCommon.Web.Core.TestWebApp/Controllers/UploadFileExtensionsController.cs).
  - [AllowUploadSafeFiles](/src/DNTCommon.Web.Core.TestWebApp/Models/UserFileViewModel.cs) attribute disallows uploading dangerous files such as .aspx, web.config and .asp files.
  - [AntiDosMiddleware](/src/DNTCommon.Web.Core.TestWebApp/Startup.cs) is a rate limiter and throttling middleware for ASP.NET Core apps. To use it first add `app.UseAntiDos()` and `services.Configure<AntiDosConfig>` to [Startup.cs](/src/DNTCommon.Web.Core.TestWebApp/Startup.cs) file. Then complete the `AntiDosConfig` section of the [appsettings.json](/src/DNTCommon.Web.Core.TestWebApp/appsettings.json) file.
  - [IAntiXssService](/src/DNTCommon.Web.Core.TestWebApp/Controllers/AntiXssController.cs) provides a cleaning service for unsafe HTML fragments that can lead to XSS attacks based on a whitelist of allowed tags and attributes. To use it add `services.Configure<AntiXssConfig>` to [Startup.cs](/src/DNTCommon.Web.Core.TestWebApp/Startup.cs) file. Then complete the `AntiXssConfig` section of the [appsettings.json](/src/DNTCommon.Web.Core.TestWebApp/appsettings.json) file.
