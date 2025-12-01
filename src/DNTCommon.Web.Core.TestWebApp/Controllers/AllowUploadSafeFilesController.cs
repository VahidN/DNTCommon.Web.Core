using System.Threading;
using System.Threading.Tasks;
using DNTCommon.Web.Core.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DNTCommon.Web.Core.TestWebApp.Controllers;

public class AllowUploadSafeFilesController : Controller
{
    private readonly IUploadFileService _uploadFileService;

    public AllowUploadSafeFilesController(IUploadFileService uploadFileService)
        => _uploadFileService = uploadFileService;

    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadGeneralFile(GeneralFileViewModel userViewModel,
        CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid)
        {
            var formFile = userViewModel.UserFile;

            var (isSaved, savedFilePath) = await _uploadFileService.SavePostedFileAsync(formFile, allowOverwrite: false,
                ["files", "nestedfolder", "nestedfolder2"], cancellationToken);

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
