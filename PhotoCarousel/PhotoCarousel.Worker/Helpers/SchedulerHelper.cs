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

namespace PhotoCarousel.Worker.Helpers
{
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
            // Get history today
            var historicPhotoIds = await _dbContext.History
                .Where(x => x.Scheduled.Date == DateTime.UtcNow.Date)
                .Select(x => x.Id)
                .ToListAsync(stoppingToken);

            // Get a random photo
            var randomPhoto = await _dbContext.Photos
                .Where(x => !string.IsNullOrEmpty(x.Description))
                .Where(x => x.Rating == Rating.ThumbsUp)
                .Where(x => !historicPhotoIds.Contains(x.Id))
                .OrderBy(x => Guid.NewGuid())
                .FirstOrDefaultAsync(stoppingToken);

            await _dbContext.History.AddAsync(new History
            {
                Id = Guid.NewGuid(),
                PhotoId = randomPhoto.Id,
                Scheduled = DateTime.UtcNow
            }, stoppingToken);
            await _dbContext.SaveChangesAsync();
        }
    }
}