using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Controllers;

[ApiController]
[Route("duplicates")]
public class DuplicatesController : ControllerBase
{
    private readonly DuplicatesService _duplicatesService;
    private readonly ILogger<DuplicatesController> _logger;

    public DuplicatesController(
        DuplicatesService duplicatesService,
        ILogger<DuplicatesController> logger)
    {
        _duplicatesService = duplicatesService;
        _logger = logger;
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetDuplicates()
    {
        var duplicates = await _duplicatesService.GetDuplicates();

        return duplicates != null ? Ok(duplicates) : NotFound();
    }
}