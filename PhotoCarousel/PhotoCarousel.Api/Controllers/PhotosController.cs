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

            return Ok(photo);
        }
    }
}