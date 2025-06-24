using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Common.Extensions;
using PhotoCarousel.DataAccess;
using PhotoCarousel.Entities;
using PhotoCarousel.Enums;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TagLib.Image;

namespace PhotoCarousel.Worker.Helpers;

public class IndexingHelper
{
    private readonly IConfiguration _configuration;
    private readonly PhotoCarouselDbContext _dbContext;
    private readonly ILogger<IndexingHelper> _logger;

    public IndexingHelper(
        IConfiguration configuration,
        PhotoCarouselDbContext dbContext,
        ILogger<IndexingHelper> logger)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Go(CancellationToken stoppingToken)
    {
        var sw = Stopwatch.StartNew();

        var photoRootPath = _configuration.GetPhotoRootPath();
        await IndexPhotos(new DirectoryInfo(photoRootPath), stoppingToken);

        sw.Stop();
        _logger.LogInformation($"Whole photo library indexed successfully: {sw.Elapsed.TotalMinutes:F0} min");
    }

    private async Task<int> IndexPhotos(DirectoryInfo directoryInfo, CancellationToken stoppingToken)
    {
        var regex = new Regex(@"[0-9]{4}-[0-9]{2}\s\(.*?\)");
        var match = regex.Match(directoryInfo.Name);
        bool isAlbumFolder = match.Success;
        int numberOfPhotosInAlbumFolder = 0;
        var numberOfPhotosIndexed = 0;

        var folderSw = Stopwatch.StartNew();

        foreach (var fileInfo in directoryInfo.GetFiles())
        {
            try
            {
                if (!fileInfo.FullName.Contains("@eaDir") && fileInfo.Extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase))
                {
                    var sw = Stopwatch.StartNew();

                    numberOfPhotosInAlbumFolder++;
                    var hash = await CalculateSha256(fileInfo, stoppingToken);

                    if (!await _dbContext.Photos.AnyAsync(
                            x => x.Sha256Hash == hash && x.SourcePath == fileInfo.FullName, stoppingToken))
                    {
                        var indexedPhoto = GenerateIndexedPhoto(fileInfo, hash);
                        numberOfPhotosIndexed++;
                        await _dbContext.Photos.AddAsync(indexedPhoto, stoppingToken);
                        await _dbContext.SaveChangesAsync(stoppingToken);

                        sw.Stop();
                        _logger.LogInformation($"Photo '{indexedPhoto.SourcePath}' indexed successfully: {sw.ElapsedMilliseconds}ms");
                    }
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
                numberOfPhotosInAlbumFolder += await IndexPhotos(childDirectoryInfo, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while indexing directory '{childDirectoryInfo.FullName}': {ex.Message}");
            }
        }

        folderSw.Stop();
        if (isAlbumFolder && numberOfPhotosIndexed > 0)
        {
            _logger.LogInformation($"Album folder '{match.Value}' with {numberOfPhotosIndexed} of {numberOfPhotosInAlbumFolder} photos indexed successfully: {Math.Round(folderSw.Elapsed.TotalSeconds)}s");
        }

        return numberOfPhotosInAlbumFolder;
    }

    private Photo GenerateIndexedPhoto(FileInfo fileInfo, byte[] hash)
    {
        var regex = new Regex(@"[0-9]{4}-[0-9]{2}\s\(.*?\)");
        var match = regex.Match(fileInfo.FullName);
        var metadata = GetMetaData(fileInfo);

        return new()
        {
            Id = Guid.NewGuid(),
            SourcePath = fileInfo.FullName,
            FolderPath = Path.GetDirectoryName(fileInfo.FullName),
            ThumbnailPath = string.Empty,
            Sha256Hash = hash,
            Rating = string.IsNullOrWhiteSpace(match.Value) ? Rating.ThumbsDown : Rating.None,
            DateTaken = metadata.Item1,
            DateIndexed = DateTime.UtcNow,
            Description = match.Value,
            Orientation = metadata.Item2
        };
    }

    private async Task<byte[]> CalculateSha256(FileInfo fileInfo, CancellationToken stoppingToken)
    {
        using SHA256 sha256 = SHA256.Create();
        var fileStream = fileInfo.Open(FileMode.Open);
        fileStream.Position = 0;
        byte[] hash = await sha256.ComputeHashAsync(fileStream, stoppingToken);
        fileStream.Close();

        return hash;
    }

    private (DateTime, Orientation) GetMetaData(FileInfo fileInfo)
    {
        var file = TagLib.File.Create(fileInfo.FullName);
        var imageTags = file.Tag as TagLib.Image.CombinedImageTag;

        try
        {
            var orientation = Orientation.Landscape;

            if (imageTags != null && imageTags.Orientation != ImageOrientation.None)
            {
                switch (imageTags.Orientation)
                {
                    case ImageOrientation.TopLeft:
                    case ImageOrientation.TopRight:
                    case ImageOrientation.BottomLeft:
                    case ImageOrientation.BottomRight:
                        orientation = Orientation.Landscape;
                        break;
                    case ImageOrientation.LeftTop:
                    case ImageOrientation.RightTop:
                    case ImageOrientation.LeftBottom:
                    case ImageOrientation.RightBottom:
                        orientation = Orientation.Portrait;
                        break;
                }
            }

            return (imageTags is { DateTime: not null } ? imageTags.DateTime.Value : fileInfo.LastWriteTime, orientation);
        }
        catch
        {
            return (fileInfo.LastWriteTime, Orientation.Landscape);
        }
    }
}