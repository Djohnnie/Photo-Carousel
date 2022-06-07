using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;
using System;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Controllers;

[ApiController]
[Route("downloads")]
public class DownloadsController : BaseController<DownloadsController>
{
    private readonly DownloadService _downloadService;

    public DownloadsController(
        DownloadService downloadService,
        ILogger<DownloadsController> logger) : base(logger)
    {
        _downloadService = downloadService;
    }

    [HttpGet("photo/{id}")]
    public async Task<IActionResult> DownloadPhotoById(Guid id)
    {
        return await Log<IActionResult>(async () =>
        {
            var stream = await _downloadService.GetPhotoStreamByPhotoId(id);

            return File(stream, "application/octet-stream", $"{id}.jpg");
        });
    }

    [HttpGet("thumbnail/{id}")]
    public async Task<IActionResult> DownloadThumbnailById(Guid id)
    {
        return await Log<IActionResult>(async () =>
        {
            var stream = await _downloadService.GetThumbnailStreamByPhotoId(id);

            return File(stream, "application/octet-stream", $"{id}.thumbnail.jpg");
        });
    }
}