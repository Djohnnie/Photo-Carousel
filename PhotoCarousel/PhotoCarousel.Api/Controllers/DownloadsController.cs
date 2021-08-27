using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;
using System;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Controllers
{
    [ApiController]
    [Route("downloads")]
    public class DownloadsController : ControllerBase
    {
        private readonly DownloadService _downloadService;
        private readonly ILogger<DownloadsController> _logger;

        public DownloadsController(
            DownloadService downloadService,
            ILogger<DownloadsController> logger)
        {
            _downloadService = downloadService;
            _logger = logger;
        }

        [HttpGet("photo/{id}")]
        public async Task<IActionResult> DownloadPhotoById(Guid id)
        {
            var stream = await _downloadService.GetPhotoStreamByPhotoId(id);

            return File(stream, "application/octet-stream", $"{id}.jpg");
        }

        [HttpGet("thumbnail/{id}")]
        public async Task<IActionResult> DownloadThumbnailById(Guid id)
        {
            var stream = await _downloadService.GetThumbnailStreamByPhotoId(id);

            return File(stream, "application/octet-stream", $"{id}.thumbnail.jpg");
        }
    }
}