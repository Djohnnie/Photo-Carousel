using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.DataAccess;
using PhotoCarousel.Entities.Enums;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
                .Take(10)
                .ToListAsync();

            var thumbnailSize = _configuration.GetThumbnailSize();
            var thumbnailPath = _configuration.GetThumbnailPath();

            foreach (var photo in photoBatch)
            {
                try
                {
                    using var fs = new FileStream(photo.SourcePath, FileMode.Open, FileAccess.Read);
                    using var sourceImage = Image.FromStream(fs, false, false);

                    var sourceBounds = photo.Orientation == Orientation.Landscape ?
                        new Rectangle(sourceImage.Width / 2 - sourceImage.Height / 2, 0, sourceImage.Height, sourceImage.Height)
                        :
                        new Rectangle(0, sourceImage.Height / 2 - sourceImage.Width / 2, sourceImage.Width, sourceImage.Width);
                    var destinationBounds = new Rectangle(0, 0, thumbnailSize, thumbnailSize);

                    using var thumbnailTarget = new Bitmap(thumbnailSize, thumbnailSize);
                    using var thumbnailGraphics = Graphics.FromImage(thumbnailTarget);
                    thumbnailGraphics.CompositingQuality = CompositingQuality.HighQuality;
                    thumbnailGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    thumbnailGraphics.CompositingMode = CompositingMode.SourceCopy;

                    thumbnailGraphics.DrawImage(
                        sourceImage, destinationBounds, sourceBounds.X, sourceBounds.Y, sourceBounds.Width, sourceBounds.Height, GraphicsUnit.Pixel);

                    var thumbnailDestinationPath = Path.Combine(thumbnailPath, $"{photo.Id}.thumbnail.jpg");
                    thumbnailTarget.Save(thumbnailDestinationPath, ImageFormat.Jpeg);

                    photo.ThumbnailPath = thumbnailDestinationPath;
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while creating thumbnail for '{photo.SourcePath}': {ex.Message}");
                }
            }
        }
    }
}