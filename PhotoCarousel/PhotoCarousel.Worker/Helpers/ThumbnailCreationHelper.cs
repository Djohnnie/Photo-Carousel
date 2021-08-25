using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.DataAccess;
using PhotoCarousel.Entities.Enums;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoCarousel.Worker.Helpers
{
    public class ThumbnailCreationHelper
    {
        private readonly IConfiguration _configuration;
        private readonly PhotoCarouselDbContext _dbContext;
        private readonly ILogger<PhotoIndexingHelper> _logger;

        public ThumbnailCreationHelper(
            IConfiguration configuration,
            PhotoCarouselDbContext dbContext,
            ILogger<PhotoIndexingHelper> logger)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Go(CancellationToken stoppingToken)
        {
            var photoBatch = await _dbContext.Photos
                .Where(x => string.IsNullOrEmpty(x.ThumbnailPath))
                .Take(16)
                .ToListAsync(stoppingToken);

            var thumbnailSize = _configuration.GetThumbnailSize();
            var thumbnailPath = _configuration.GetThumbnailPath();

            var tasks = photoBatch.Select(async photo =>
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    await using var sourceFileStream = new FileStream(photo.SourcePath, FileMode.Open, FileAccess.Read);
                    using var sourceData = SKData.Create(sourceFileStream);
                    using var sourceBitmap = SKBitmap.Decode(sourceFileStream);

                    var sourceBounds = photo.Orientation == Orientation.Landscape ?
                        SKRectI.Create(sourceBitmap.Width / 2 - sourceBitmap.Height / 2, 0, sourceBitmap.Height, sourceBitmap.Height)
                        :
                        SKRectI.Create(0, sourceBitmap.Height / 2 - sourceBitmap.Width / 2, sourceBitmap.Width, sourceBitmap.Width);

                    var destinationBounds = photo.Orientation == Orientation.Landscape ?
                        SKRectI.Create(0, 0, sourceBitmap.Height, sourceBitmap.Height)
                        :
                        SKRectI.Create(0, 0, sourceBitmap.Width, sourceBitmap.Width);

                    using var thumbnailTarget = new SKBitmap(destinationBounds.Width, destinationBounds.Height);
                    using var canvasTarget = new SKCanvas(thumbnailTarget);
                    canvasTarget.DrawBitmap(sourceBitmap, sourceBounds, destinationBounds);
                    using var destinationBitmap = thumbnailTarget.Resize(new SKSizeI(thumbnailSize, thumbnailSize), SKFilterQuality.High);

                    var thumbnailDestinationPath = Path.Combine(thumbnailPath, $"{photo.Id}.thumbnail.jpg");

                    await using var destinationFileStream = new FileStream(thumbnailDestinationPath, FileMode.CreateNew);
                    destinationBitmap.Encode(destinationFileStream, SKEncodedImageFormat.Jpeg, 90);

                    photo.ThumbnailPath = thumbnailDestinationPath;

                    sw.Stop();
                    _logger.LogInformation($"Thumbnail creation successful: {sw.ElapsedMilliseconds}ms");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while creating thumbnail for '{photo.SourcePath}': {ex}");
                }
            });

            await Task.WhenAll(tasks);

            await _dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}