using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.Worker.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoCarousel.Worker.Workers
{
    public class SchedulerWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SchedulerWorker> _logger;

        public SchedulerWorker(
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<SchedulerWorker> logger)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SCHEDULER-WORKER WILL START IN ONE MINUTE...");

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            
            _logger.LogInformation("SCHEDULER-WORKER HAS STARTED");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var serviceScope = _serviceScopeFactory.CreateScope();
                    var schedulerHelper = serviceScope.ServiceProvider.GetService<SchedulerHelper>();

                    if (schedulerHelper != null)
                    {
                        //await schedulerHelper.Go(stoppingToken);
                    }
                    else
                    {
                        _logger.LogCritical("SCHEDULER-HELPER COULD NOT BE CONSTRUCTED!!!");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Unknown error occurred while indexing: ({ex.Message}).");
                }

                var interval = _configuration.GetSchedulerIntervalInSeconds();
                await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
            }
        }
    }
}