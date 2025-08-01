﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Api.Services;
using PhotoCarousel.Contracts;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Controllers;

[ApiController]
[Route("flags")]
public class FlagsController : BaseController<FlagsController>
{
    private readonly FlagsService _flagsService;

    public FlagsController(
        FlagsService flagsService,
        ILogger<FlagsController> logger) : base(logger)
    {
        _flagsService = flagsService;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetFlag(string name)
    {
        return await Log<IActionResult>(async () =>
        {
            var flag = await _flagsService.GetFlag(name);

            return flag is not null ? Ok(flag) : NotFound();
        });
    }

    [HttpPost("set")]
    public async Task<IActionResult> SetFlag(Flag flag)
    {
        return await Log<IActionResult>(async () =>
        {
            await _flagsService.SetFlag(flag.Name, flag.Value);

            return Ok();
        });
    }
}