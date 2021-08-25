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
                .Take(4)
                .ToListAsync(stoppingToken);

            var thumbnailSize = _configuration.GetThumbnailSize();
            var thumbnailPath = _configuration.GetThumbnailPath();

            var tasks = photoBatch.Select(async photo =>
            {
                try
                {
                    var sw = Stopwatch.StartNew();

                    await using var sourceFileStream = new FileStream(photo.SourcePath, FileMode.Open, FileAccess.Read);
                    using var sourceBitmap = SKBitmap.Decode(sourceFileStream);
                    using var sourceImage = SKImage.FromBitmap(sourceBitmap);

                    var sourceBounds = photo.Orientation == Orientation.Landscape ?
                        new SKRectI(sourceImage.Width / 2 - sourceImage.Height / 2, 0, sourceImage.Height, sourceImage.Height)
                        :
                        new SKRectI(0, sourceImage.Height / 2 - sourceImage.Width / 2, sourceImage.Width, sourceImage.Width);

                    var destinationBounds = photo.Orientation == Orientation.Landscape ?
                        new SKRectI(0, 0, sourceImage.Height, sourceImage.Height)
                        :
                        new SKRectI(0, 0, sourceImage.Width, sourceImage.Width);

                    using var thumbnailTarget = new SKBitmap(destinationBounds.Width, destinationBounds.Height);
                    using var canvasTarget = new SKCanvas(thumbnailTarget);
                    canvasTarget.DrawImage(sourceImage, sourceBounds, destinationBounds);
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
                    _logger.LogError($"Error while creating thumbnail for '{photo.SourcePath}': {ex.Message}");
                }
            });

            await Task.WhenAll(tasks);

            await _dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}