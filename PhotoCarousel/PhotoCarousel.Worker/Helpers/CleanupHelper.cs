using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhotoCarousel.DataAccess;

namespace PhotoCarousel.Worker.Helpers;

public class CleanupHelper
{
    private readonly PhotoCarouselDbContext _dbContext;
    private readonly ILogger<CleanupHelper> _logger;

    public CleanupHelper(
        PhotoCarouselDbContext dbContext,
        ILogger<CleanupHelper> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Go(CancellationToken stoppingToken)
    {
        var sw = Stopwatch.StartNew();

        var recordsRemoved = await _dbContext.History.Where(x => x.Scheduled < DateTime.UtcNow.Date.AddDays(28)).ExecuteDeleteAsync();

        sw.Stop();
        _logger.LogInformation($"{recordsRemoved} history records cleaned successfully: {sw.Elapsed.TotalSeconds:F0}s");
    }
}