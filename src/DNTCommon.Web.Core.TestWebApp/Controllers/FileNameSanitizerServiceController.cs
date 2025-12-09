using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class FileNameSanitizerServiceController : Controller
{
    private readonly IFileNameSanitizerService _fileNameSanitizerService;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public FileNameSanitizerServiceController(IWebHostEnvironment hostingEnvironment,
        IFileNameSanitizerService fileNameSanitizerService)
    {
        _fileNameSanitizerService = fileNameSanitizerService;
        _hostingEnvironment = hostingEnvironment;
    }

    public IActionResult Index() => View();

    public IActionResult Download(string fileName)
    {
        // To avoid `Directory Traversal` & `File Inclusion` attacks, never use the requested `fileName` directly.

        var filesFolder = _hostingEnvironment.WebRootPath.SafePathCombine("files");
        var cleanedFile = _fileNameSanitizerService.IsSafeToDownload(filesFolder, fileName);

        if (!cleanedFile.IsSafeToDownload)
        {
            return BadRequest();
        }

        return PhysicalFile(cleanedFile.SafeFilePath, contentType: "application/octet-stream",
            cleanedFile.SafeFileName);
    }
}
