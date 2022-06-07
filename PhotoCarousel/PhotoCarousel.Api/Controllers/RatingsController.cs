using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;
using PhotoCarousel.Contracts;

namespace PhotoCarousel.Api.Controllers;

[ApiController]
[Route("ratings")]
public class RatingsController : BaseController<RatingsController>
{
    private readonly RatingService _ratingService;

    public RatingsController(
        RatingService ratingService,
        ILogger<RatingsController> logger) : base(logger)
    {
        _ratingService = ratingService;
    }

    [HttpPost]
    public async Task<IActionResult> SetRating(PhotoRating photoRating)
    {
        return await Log<IActionResult>(async () =>
        {
            await _ratingService.SetRating(photoRating);

            return Ok();
        });
    }
}