using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.DataAccess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoCarousel.Enums;
using PhotoCarousel.Entities;
using PhotoCarousel.Common.Extensions;

namespace PhotoCarousel.Worker.Helpers;

public class SchedulerHelper
{
    private readonly IConfiguration _configuration;
    private readonly PhotoCarouselDbContext _dbContext;
    private readonly ILogger<SchedulerHelper> _logger;

    public SchedulerHelper(
        IConfiguration configuration,
        PhotoCarouselDbContext dbContext,
        ILogger<SchedulerHelper> logger)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Go(CancellationToken stoppingToken)
    {
        var slideshowDuration = _configuration.GetPhotoSlideshowDuration();

        // Get history today
        var historicPhotoIds = await _dbContext.History
            .Where(x => x.Scheduled.Date == DateTime.UtcNow.Date)
            .Select(x => x.Id)
            .ToListAsync(stoppingToken);

        // Get random photos
        var randomPhotos = await _dbContext.Photos
            .Where(x => !string.IsNullOrEmpty(x.Description))
            .Where(x => x.Rating == Rating.ThumbsUp)
            .Where(x => !historicPhotoIds.Contains(x.Id))
            .OrderBy(x => Guid.NewGuid())
            .Take(2).ToListAsync();

        // Get the final two historic photos.
        var historicPhotos = await _dbContext.History
            .OrderByDescending(x => x.SysId)
            .Take(2).ToListAsync();

        // If there are no historic photos,
        // Schedule a current one and a next one.q
        if (!historicPhotos.Any())
        {
            var utcNow = DateTime.UtcNow.RoundToMinutes();

            var currentPhoto = new History
            {
                Id = Guid.NewGuid(),
                PhotoId = randomPhotos[0].Id,
                Scheduled = utcNow
            };

            await _dbContext.History.AddAsync(currentPhoto);

            var nextPhoto = new History
            {
                Id = Guid.NewGuid(),
                PhotoId = randomPhotos[1].Id,
                Scheduled = utcNow.AddMinutes(slideshowDuration)
            };

            await _dbContext.History.AddAsync(nextPhoto);

            await _dbContext.SaveChangesAsync();

            return;
        }

        var lastHistoricPhoto = historicPhotos.First();

        if (lastHistoricPhoto.Scheduled < DateTime.UtcNow)
        {
            var nextPhoto = new History
            {
                Id = Guid.NewGuid(),
                PhotoId = randomPhotos[0].Id,
                Scheduled = lastHistoricPhoto.Scheduled.AddMinutes(slideshowDuration)
            };

            await _dbContext.History.AddAsync(nextPhoto);

            await _dbContext.SaveChangesAsync();

            return;
        }
    }
}