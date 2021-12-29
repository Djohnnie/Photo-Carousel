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

public class CleanupWorker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CleanupWorker> _logger;

    public CleanupWorker(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<CleanupWorker> logger)
    {
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CLEANUP-WORKER WILL START IN ONE MINUTE...");

        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        _logger.LogInformation("CLEANUP-WORKER HAS STARTED");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var serviceScope = _serviceScopeFactory.CreateScope();
                var cleanupHelper = serviceScope.ServiceProvider.GetService<CleanupHelper>();

                if (cleanupHelper != null)
                {
                    await cleanupHelper.Go(stoppingToken);
                }
                else
                {
                    _logger.LogCritical("CLEANUP-HELPER COULD NOT BE CONSTRUCTED!!!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unknown error occurred while cleaning up: ({ex.Message}).");
            }

            var interval = _configuration.GetCleanupIntervalInSeconds();
            await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
        }
    }
}