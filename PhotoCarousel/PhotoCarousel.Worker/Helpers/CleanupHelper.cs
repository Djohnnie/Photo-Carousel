using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhotoCarousel.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        _logger.LogInformation($"{recordsRemoved} history records cleaned successfully: {sw.Elapsed.TotalMilliseconds:F0}ms");

        sw = Stopwatch.StartNew();

        var photosToRemove = new List<Guid>(await _dbContext.Photos.CountAsync());

        foreach (var photo in await _dbContext.Photos.AsNoTracking().Select(x => new { x.Id, x.SourcePath }).ToListAsync())
        {
            var fullPath = photo.SourcePath.Replace("/", @"\");
            if (!File.Exists(fullPath))
            {
                photosToRemove.Add(photo.Id);
            }
        }

        //await _dbContext.Photos.Where(x => photosToRemove.Contains(x.Id)).ExecuteDeleteAsync();

        sw.Stop();
        _logger.LogInformation($"{photosToRemove.Count} photos that do not exist removed from database successfully: {sw.Elapsed.TotalSeconds:F0}s");
    }
}