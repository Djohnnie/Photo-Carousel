using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;

namespace PhotoCarousel.Api.Controllers;

[ApiController]
[Route("folders")]
public class FoldersController : BaseController<FoldersController>
{
    private readonly FolderService _folderService;

    public FoldersController(
        FolderService folderService,
        ILogger<FoldersController> logger) : base(logger)
    {
        _folderService = folderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFolders()
    {
        return await Log<IActionResult>(async () =>
        {
            var folders = await _folderService.GetFolders();

            return folders != null ? Ok(folders) : NotFound();
        });
    }
}