using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;
using PhotoCarousel.Contracts;

namespace PhotoCarousel.Api.Controllers;

[ApiController]
[Route("photos")]
public class RatingsController : ControllerBase
{
    private readonly RatingService _ratingService;
    private readonly ILogger<RatingsController> _logger;

    public RatingsController(
        RatingService ratingService,
        ILogger<RatingsController> logger)
    {
        _ratingService = ratingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SetRating(PhotoRating photoRating)
    {
        await _ratingService.SetRating(photoRating);

        return Ok();
    }
}