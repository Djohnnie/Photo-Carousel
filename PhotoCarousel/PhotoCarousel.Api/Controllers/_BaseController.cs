using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Controllers;

public class BaseController<TController> : ControllerBase
{
    private readonly ILogger<TController> _logger;

    protected BaseController(ILogger<TController> logger)
    {
        _logger = logger;
    }

    protected async Task<TResult> Log<TResult>(Func<Task<TResult>> f, [CallerMemberName] string description = "<unknown>")
    {
        var stopwatch = Stopwatch.StartNew();

        var result = await f();

        _logger.LogInformation($"Request '{description}' took {stopwatch.ElapsedMilliseconds:N0} ms");

        return result;
    }
}