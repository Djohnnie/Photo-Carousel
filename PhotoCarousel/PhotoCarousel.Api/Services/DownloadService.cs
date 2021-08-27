using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.DataAccess;
using PhotoCarousel.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Services
{
    public class DownloadService
    {
        private readonly IConfiguration _configuration;
        private readonly PhotoCarouselDbContext _dbContext;
        private readonly ILogger<DownloadService> _logger;

        public DownloadService(
            IConfiguration configuration,
            PhotoCarouselDbContext dbContext,
            ILogger<DownloadService> logger)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
        }

        public Task<Stream> GetPhotoStreamByPhotoId(Guid id)
        {
            return GetStreamByPhotoId(id, photo => photo.SourcePath);
        }

        public Task<Stream> GetThumbnailStreamByPhotoId(Guid id)
        {
            return GetStreamByPhotoId(id, photo => photo.ThumbnailPath);
        }

        private async Task<Stream> GetStreamByPhotoId(Guid id, Func<Photo, string> imageSource)
        {
            var photo = await _dbContext.Photos.SingleOrDefaultAsync(x => x.Id == id);

            if (photo != null)
            {
                return new FileStream(imageSource(photo), FileMode.Open, FileAccess.Read);
            }

            return Stream.Null;
        }
    }
}