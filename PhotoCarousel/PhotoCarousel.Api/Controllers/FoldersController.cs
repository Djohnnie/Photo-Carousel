using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;

namespace PhotoCarousel.Api.Controllers
{
    [ApiController]
    [Route("folders")]
    public class FoldersController : ControllerBase
    {
        private readonly FolderService _folderService;
        private readonly ILogger<FoldersController> _logger;

        public FoldersController(
            FolderService folderService,
            ILogger<FoldersController> logger)
        {
            _folderService = folderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {
            var folders = await _folderService.GetFolders();

            return folders != null ? Ok(folders) : NotFound();
        }
    }
}