using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PhotoCarousel.Entities;
using PhotoCarousel.Enums;
using PhotoContract = PhotoCarousel.Contracts.Photo;

namespace PhotoCarousel.Api.Services
{
    public class PhotoService
    {
        private readonly IConfiguration _configuration;
        private readonly PhotoCarouselDbContext _dbContext;
        private readonly ILogger<PhotoService> _logger;

        public PhotoService(
            IConfiguration configuration,
            PhotoCarouselDbContext dbContext,
            ILogger<PhotoService> logger)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<PhotoContract> GetRandomPhoto()
        {
            Expression<Func<Photo, bool>> predicate = photo =>
                !string.IsNullOrEmpty(photo.Description) &&
                photo.Rating != Rating.ThumbsDown &&
                photo.Orientation == Orientation.Landscape;

            var count = await _dbContext.Photos.Where(predicate).CountAsync();
            var skip = new Random().Next(0, count);
            var photo = await _dbContext.Photos.Where(predicate).Skip(skip).FirstOrDefaultAsync();

            return new()
            {
                Id = photo.Id,
                Description = photo.Description,
                Rating = photo.Rating
            };
        }

        public async Task<List<PhotoContract>> GetPhotosByFolder(string folderPath)
        {
            var photos = await _dbContext.Photos.Where(x => x.FolderPath == folderPath).ToListAsync();

            return photos.Select(photo => new PhotoContract
            {
                Id = photo.Id,
                Description = Path.GetFileName(photo.SourcePath),
                Rating = photo.Rating
            }).ToList();
        }
    }
}