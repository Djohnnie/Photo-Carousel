using System.Diagnostics;
using System.IO;
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

        var photoIds = await _dbContext.Photos.Select(x => x.Id).ToListAsync(stoppingToken);

        foreach (var photoId in photoIds)
        {
            var photo = await _dbContext.Photos.SingleOrDefaultAsync(x => x.Id == photoId, stoppingToken);

            if (!File.Exists(photo.SourcePath))
            {
                _dbContext.Photos.Remove(photo);
                await _dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation($"Photo '{photo.SourcePath}' does not exist and was cleaned.");
            }
        }

        sw.Stop();
        _logger.LogInformation($"Whole photo library cleaned successfully: {sw.Elapsed.TotalSeconds:F0} sec");
    }
}