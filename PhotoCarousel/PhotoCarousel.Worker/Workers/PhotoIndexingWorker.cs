using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Worker.Helpers;

namespace PhotoCarousel.Worker.Workers
{
    public class PhotoIndexingWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PhotoIndexingWorker> _logger;

        public PhotoIndexingWorker(
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<PhotoIndexingWorker> logger)
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
                _logger.LogInformation("Worker running at: {time}", DateTime.Now);

                using var serviceScope = _serviceScopeFactory.CreateScope();
                var indexingHelper = serviceScope.ServiceProvider.GetService<PhotoIndexingHelper>();

                if (indexingHelper != null)
                {
                    await indexingHelper.Go(stoppingToken);
                }
                else
                {
                    _logger.LogCritical("PHOTO-INDEXING-HELPER COULD NOT BE CONSTRUCTED!!!");
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}