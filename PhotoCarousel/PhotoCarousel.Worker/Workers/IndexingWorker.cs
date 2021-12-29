using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.Worker.Helpers;

namespace PhotoCarousel.Worker.Workers;

public class IndexingWorker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IndexingWorker> _logger;

    public IndexingWorker(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<IndexingWorker> logger)
    {
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("INDEXING-WORKER WILL START IN ONE MINUTE...");

        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            
        _logger.LogInformation("INDEXING-WORKER HAS STARTED");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var serviceScope = _serviceScopeFactory.CreateScope();
                var indexingHelper = serviceScope.ServiceProvider.GetService<IndexingHelper>();

                if (indexingHelper != null)
                {
                    await indexingHelper.Go(stoppingToken);
                }
                else
                {
                    _logger.LogCritical("PHOTO-INDEXING-HELPER COULD NOT BE CONSTRUCTED!!!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unknown error occurred while indexing: ({ex.Message}).");
            }

            var interval = _configuration.GetIndexerIntervalInSeconds();
            await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
        }
    }
}