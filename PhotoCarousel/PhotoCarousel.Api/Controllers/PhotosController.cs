using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Controllers
{
    [ApiController]
    [Route("photos")]
    public class PhotosController : ControllerBase
    {
        private readonly PhotoService _photoService;
        private readonly ILogger<PhotosController> _logger;

        public PhotosController(
            PhotoService photoService,
            ILogger<PhotosController> logger)
        {
            _photoService = photoService;
            _logger = logger;
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomPhoto()
        {
            var photo = await _photoService.GetRandomPhoto();

            return photo != null ? Ok(photo) : NotFound();
        }

        [HttpGet("byfolder")]
        public async Task<IActionResult> GetPhotosByFolder(string folderPath)
        {
            var photos = await _photoService.GetPhotosByFolder(folderPath);

            return Ok(photos);
        }

        [HttpGet("previous")]
        public async Task<IActionResult> GetPreviousPhoto()
        {
            var photo = await _photoService.GetPreviousPhoto();

            return photo != null ? Ok(photo) : NotFound();
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentPhoto()
        {
            var photo = await _photoService.GetCurrentPhoto();

            return photo != null ? Ok(photo) : NotFound();
        }

        [HttpGet("next")]
        public async Task<IActionResult> GetNextPhoto()
        {
            var photo = await _photoService.GetNextPhoto();

            return photo != null ? Ok(photo) : NotFound();
        }
    }
}