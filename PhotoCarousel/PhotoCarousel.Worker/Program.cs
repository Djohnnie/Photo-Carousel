using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhotoCarousel.DataAccess;
using PhotoCarousel.Worker.Helpers;
using PhotoCarousel.Worker.Workers;

namespace PhotoCarousel.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configBuilder =>
                {
                    configBuilder.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<PhotoCarouselDbContext>();
                    services.AddTransient<IndexingHelper>();
                    services.AddTransient<ThumbnailCreationHelper>();
                    services.AddTransient<SchedulerHelper>();
                    services.AddHostedService<StartupWorker>();
                    services.AddHostedService<IndexingWorker>();
                    services.AddHostedService<ThumbnailCreationWorker>();
                    services.AddHostedService<SchedulerWorker>();
                });
    }
}