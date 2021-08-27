using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.Worker.Helpers;

namespace PhotoCarousel.Worker.Workers
{
    public class ThumbnailCreationWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ThumbnailCreationWorker> _logger;

        public ThumbnailCreationWorker(
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ThumbnailCreationWorker> logger)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                int numberOfThumbnailsCreated = 0;

                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTime.Now);

                    using var serviceScope = _serviceScopeFactory.CreateScope();
                    var thumbnailHelper = serviceScope.ServiceProvider.GetService<ThumbnailCreationHelper>();

                    if (thumbnailHelper != null)
                    {
                        numberOfThumbnailsCreated = await thumbnailHelper.Go(stoppingToken);
                    }
                    else
                    {
                        _logger.LogCritical("THUMBNAIL-CREATION-HELPER COULD NOT BE CONSTRUCTED!!!");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Unknown error occurred while creating thumbnails: ({ex.Message}).");
                }

                if(numberOfThumbnailsCreated > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
                else
                {
                    var interval = _configuration.GetThumbnailerIntervalInSeconds();
                    await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
                }
            }
        }
    }
}