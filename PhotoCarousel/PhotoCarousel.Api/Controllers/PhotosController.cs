using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;
using PhotoCarousel.Contracts;
using System;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Controllers;

[ApiController]
[Route("photos")]
public class PhotosController : BaseController<PhotosController>
{
    private readonly PhotoService _photoService;

    public PhotosController(
        PhotoService photoService,
        ILogger<PhotosController> logger) : base(logger)
    {
        _photoService = photoService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhoto(Guid id)
    {
        return await Log<IActionResult>(async () =>
        {
            var photo = await _photoService.GetPhotoById(id);

            return photo != null ? Ok(photo) : NotFound();
        });
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandomPhoto()
    {
        return await Log<IActionResult>(async () =>
        {
            var photo = await _photoService.GetRandomPhoto();

            return photo != null ? Ok(photo) : NotFound();
        });
    }

    [HttpGet("byfolder")]
    public async Task<IActionResult> GetPhotosByFolder(string folderPath)
    {
        return await Log<IActionResult>(async () =>
        {
            var photos = await _photoService.GetPhotosByFolder(folderPath);

            return Ok(photos);
        });
    }

    [HttpGet("previous")]
    public async Task<IActionResult> GetPreviousPhoto()
    {
        return await Log<IActionResult>(async () =>
        {
            var photo = await _photoService.GetPreviousPhoto();

            return photo != null ? Ok(photo) : NotFound();
        });
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentPhoto()
    {
        return await Log<IActionResult>(async () =>
        {
            var photo = await _photoService.GetCurrentPhoto();

            return photo != null ? Ok(photo) : NotFound();
        });
    }

    [HttpGet("next")]
    public async Task<IActionResult> GetNextPhoto()
    {
        return await Log<IActionResult>(async () =>
        {
            var photo = await _photoService.GetNextPhoto();

            return photo != null ? Ok(photo) : NotFound();
        });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] PhotosToDelete request)
    {
        return await Log<IActionResult>(async () =>
        {
            await _photoService.DeletePhotos(request.PhotoIds);

            return Ok();
        });
    }
}