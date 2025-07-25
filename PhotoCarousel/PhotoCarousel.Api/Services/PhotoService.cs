﻿using Microsoft.EntityFrameworkCore;
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
using PhotoCarousel.Common.Extensions;

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

        public async Task<PhotoContract> GetPhotoById(Guid photoId)
        {
            try
            {
                var photo = await _dbContext.Photos.SingleOrDefaultAsync(x => x.Id == photoId);

                if (photo != null && File.Exists(photo.SourcePath))
                {
                    var info = await GetInfo(photo);

                    return new PhotoContract
                    {
                        Id = photo.Id,
                        Description = photo.Description,
                        Rating = photo.Rating,
                        Info = info
                    };
                }
            }
            catch { }

            return null;
        }

        public async Task<PhotoContract> GetRandomPhoto()
        {
            bool error;

            do
            {
                try
                {
                    Expression<Func<Photo, bool>> predicate = photo =>
                        !string.IsNullOrEmpty(photo.Description) &&
                        photo.Rating != Rating.ThumbsDown &&
                        photo.Orientation == Orientation.Landscape;

                    var count = await _dbContext.Photos.Where(predicate).CountAsync();
                    var skip = new Random().Next(0, count);
                    var photo = await _dbContext.Photos.Where(predicate).Skip(skip).FirstOrDefaultAsync();

                    if (File.Exists(photo.SourcePath))
                    {
                        return new()
                        {
                            Id = photo.Id,
                            Description = photo.Description,
                            Rating = photo.Rating
                        };
                    }

                    error = true;
                }
                catch
                {
                    error = true;
                    await Task.Delay(100);
                }

            } while (error);

            return null;
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

        public async Task<PhotoContract> GetPreviousPhoto()
        {
            var slideshowDuration = _configuration.GetPhotoSlideshowDuration();

            var historicPhoto = await _dbContext.History
                .OrderByDescending(x => x.SysId)
                .Where(x => x.Scheduled < DateTime.UtcNow.AddMinutes(-slideshowDuration))
                .Where(x => x.Scheduled > DateTime.UtcNow.AddMinutes(-slideshowDuration * 2))
                .FirstOrDefaultAsync();

            return await GetPhoto(historicPhoto);
        }

        public async Task<PhotoContract> GetCurrentPhoto()
        {
            var slideshowDuration = _configuration.GetPhotoSlideshowDuration();

            var historicPhoto = await _dbContext.History
                .OrderByDescending(x => x.SysId)
                .Where(x => x.Scheduled < DateTime.UtcNow)
                .Where(x => x.Scheduled > DateTime.UtcNow.AddMinutes(-slideshowDuration))
                .FirstOrDefaultAsync();

            return await GetPhoto(historicPhoto);
        }

        public async Task DeletePhotos(Guid[] photoIds)
        {
            var photosToRemove = await _dbContext.Photos.Where(x => photoIds.Contains(x.Id)).ToListAsync();

            foreach (var photo in photosToRemove)
            {
                try
                {
                    if (File.Exists(photo.SourcePath))
                    {
                        File.Delete(photo.SourcePath);
                    }

                    _dbContext.Remove(photo);
                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    // Nothing we can do...
                }
            }
        }

        public async Task<PhotoContract> GetNextPhoto()
        {
            var historicPhoto = await _dbContext.History
                .OrderByDescending(x => x.SysId)
                .Where(x => x.Scheduled > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            return await GetPhoto(historicPhoto);
        }

        private async Task<PhotoContract> GetPhoto(History historicPhoto)
        {
            if (historicPhoto != null)
            {
                var photo = await _dbContext.Photos.SingleOrDefaultAsync(x => x.Id == historicPhoto.PhotoId);

                var info = await GetInfo(photo);

                return new PhotoContract
                {
                    Id = photo.Id,
                    Description = photo.Description,
                    Rating = photo.Rating,
                    Info = info
                };
            }

            return null;
        }

        private async Task<string> GetInfo(Photo photo)
        {
            var info = string.Empty;

            if (photo != null && !string.IsNullOrEmpty(photo.FolderPath))
            {
                var infoFilePath = $"{photo.FolderPath}/info.nfo";

                if (File.Exists(infoFilePath))
                {
                    info = await File.ReadAllTextAsync(infoFilePath);
                }
            }

            return info;
        }
    }
}