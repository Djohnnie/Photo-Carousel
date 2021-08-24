using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.DataAccess;
using PhotoCarousel.Entities;
using PhotoCarousel.Entities.Enums;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoCarousel.Worker.Helpers
{
    public class PhotoIndexingHelper
    {
        private readonly IConfiguration _configuration;
        private readonly PhotoCarouselDbContext _dbContext;
        private readonly ILogger<PhotoIndexingHelper> _logger;

        public PhotoIndexingHelper(
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
            var photoRootPath = _configuration.GetPhotoRootPath();
            await IndexPhotos(new DirectoryInfo(photoRootPath));
        }


        private async Task IndexPhotos(DirectoryInfo directoryInfo)
        {
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                try
                {
                    if (!fileInfo.FullName.Contains("@eaDir") && fileInfo.Extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var sw = Stopwatch.StartNew();

                        var hash = await CalculateSha256(fileInfo);

                        if (!await _dbContext.Photos.AnyAsync(
                            x => x.Sha256Hash == hash && x.SourcePath == directoryInfo.FullName))
                        {
                            var indexedPhoto = GenerateIndexedPhoto(fileInfo, hash);

                            await _dbContext.Photos.AddAsync(indexedPhoto);
                            await _dbContext.SaveChangesAsync();
                        }

                        sw.Stop();
                        _logger.LogInformation($"STOPWATCH: {sw.ElapsedMilliseconds}ms");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while indexing file '{fileInfo.FullName}': {ex.Message}");
                }
            }

            foreach (var childDirectoryInfo in directoryInfo.GetDirectories())
            {
                try
                {
                    await IndexPhotos(childDirectoryInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while indexing directory '{childDirectoryInfo.FullName}': {ex.Message}");
                }
            }
        }

        private Photo GenerateIndexedPhoto(FileInfo fileInfo, byte[] hash)
        {
            var regex = new Regex(@"[0-9]{4}-[0-9]{2}\s\(.*\)");
            var match = regex.Match(fileInfo.FullName);
            var metadata = GetDateTaken(fileInfo);

            return new()
            {
                Id = Guid.NewGuid(),
                SourcePath = fileInfo.FullName,
                Sha256Hash = hash,
                Rating = Rating.None,
                DateTaken = metadata.Item1,
                Description = match.Value,
                Orientation = metadata.Item2
            };
        }

        private async Task<byte[]> CalculateSha256(FileInfo fileInfo)
        {
            using SHA256 sha256 = SHA256.Create();
            var fileStream = fileInfo.Open(FileMode.Open);
            fileStream.Position = 0;
            byte[] hash = await sha256.ComputeHashAsync(fileStream);
            fileStream.Close();

            return hash;
        }

        private (DateTime, Orientation) GetDateTaken(FileInfo fileInfo)
        {
            Regex r = new Regex(":");
            using var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            using Image myImage = Image.FromStream(fs, false, false);
            var orientation = myImage.Width > myImage.Height ? Orientation.Landscape : Orientation.Portrait;

            try
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                if (propItem != null)
                {
                    var str = Encoding.UTF8.GetString(propItem.Value);
                    string dateTaken = r.Replace(str, "-", 2);
                    return (DateTime.Parse(dateTaken), orientation);
                }
            }
            catch { }

            return (fileInfo.LastWriteTime, orientation);
        }
    }
}