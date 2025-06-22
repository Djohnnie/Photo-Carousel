using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.DataAccess;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TagLib.Image;

namespace PhotoCarousel.Worker.Workers;

internal class OrientationVerifierWorker : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OrientationVerifierWorker> _logger;

    public OrientationVerifierWorker(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<OrientationVerifierWorker> logger)
    {
        _configuration = configuration;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var serviceScope = _serviceScopeFactory.CreateScope();
                using var dbContext = serviceScope.ServiceProvider.GetService<PhotoCarouselDbContext>();

                bool notDone = true;
                var skip = 102000;

                var total = await dbContext.Photos.CountAsync(stoppingToken);

                while (notDone)
                {
                    Console.WriteLine($"Processing photos: {skip}/{total}");
                    try
                    {
                        var toDo = await dbContext.Photos.Skip(skip).Take(10).ToListAsync();
                        skip += 10;
                        notDone = toDo.Any();
                        foreach (var photo in toDo)
                        {
                            try
                            {
                                var photoPath = $"\\\\192.168.10.1{photo.SourcePath.Replace("/", "\\")}";
                                var file = TagLib.File.Create(photoPath);
                                var imageTags = file.Tag as CombinedImageTag;

                                var orientation = photo.Orientation;

                                if (imageTags != null && imageTags.Orientation != ImageOrientation.None)
                                {
                                    switch (imageTags.Orientation)
                                    {
                                        case ImageOrientation.TopLeft:
                                        case ImageOrientation.TopRight:
                                        case ImageOrientation.BottomLeft:
                                        case ImageOrientation.BottomRight:
                                            orientation = Enums.Orientation.Landscape;
                                            break;
                                        case ImageOrientation.LeftTop:
                                        case ImageOrientation.RightTop:
                                        case ImageOrientation.LeftBottom:
                                        case ImageOrientation.RightBottom:
                                            orientation = Enums.Orientation.Portrait;
                                            break;
                                    }
                                }

                                if (orientation != photo.Orientation)
                                {
                                    _logger.LogInformation($"Updating orientation for photo: {photo.Description} from {photo.Orientation} to {orientation}");

                                    photo.Orientation = orientation;
                                    await dbContext.SaveChangesAsync(stoppingToken);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred while reading the image orientation.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error while processing photos: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unknown error occurred while indexing: ({ex.Message}).");
                _logger.LogCritical($"{ex}");
            }

            var interval = _configuration.GetSchedulerIntervalInSeconds();
            await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
        }
    }
}