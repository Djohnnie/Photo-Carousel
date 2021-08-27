using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.DataAccess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoCarousel.Entities.Enums;

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
            var historyToday = await _dbContext.History
                .Where(x => x.Scheduled.Date == DateTime.Today)
                .ToListAsync(stoppingToken);
            var idsToExclude = historyToday.Select(x => x.PhotoId);

            // Get a random photo
            var randomPhoto = await _dbContext.Photos
                .Where(x => !string.IsNullOrEmpty(x.Description))
                .Where(x => x.Rating == Rating.ThumbsUp)
                .Where(x => !idsToExclude.Contains(x.Id))
                .ToListAsync(stoppingToken);
        }
    }
}