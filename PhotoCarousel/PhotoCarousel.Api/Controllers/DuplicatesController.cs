using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Controllers;

[ApiController]
[Route("duplicates")]
public class DuplicatesController : BaseController<DuplicatesController>
{
    private readonly DuplicatesService _duplicatesService;

    public DuplicatesController(
        DuplicatesService duplicatesService,
        ILogger<DuplicatesController> logger) : base(logger)
    {
        _duplicatesService = duplicatesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDuplicates()
    {
        return await Log<IActionResult>(async () =>
        {
            var duplicates = await _duplicatesService.GetDuplicates();

            return duplicates != null ? Ok(duplicates) : NotFound();
        });
    }
}