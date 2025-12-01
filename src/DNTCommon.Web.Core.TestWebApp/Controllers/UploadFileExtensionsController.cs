using System.Threading;
using System.Threading.Tasks;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class UploadFileExtensionsController : Controller
{
    private readonly IUploadFileService _uploadFileService;

    public UploadFileExtensionsController(IUploadFileService uploadFileService)
        => _uploadFileService = uploadFileService;

    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPhoto(UserFileViewModel userViewModel,
        CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid)
        {
            var formFile = userViewModel.Photo;

            var (isSaved, savedFilePath) = await _uploadFileService.SavePostedFileAsync(formFile, allowOverwrite: false,
                ["images", "nestedfolder", "nestedfolder2"], cancellationToken);

            if (!isSaved)
            {
                ModelState.AddModelError(key: "", errorMessage: "Uploaded file is null or empty.");

                return View(viewName: "Index");
            }

            RedirectToAction(actionName: "Index");
        }

        return View(viewName: "Index");
    }
}
