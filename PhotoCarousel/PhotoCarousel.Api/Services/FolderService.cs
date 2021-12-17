using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.DataAccess;
using FolderContract = PhotoCarousel.Api.Contracts.Folder;

namespace PhotoCarousel.Api.Services;

public class FolderService
{
    private readonly IConfiguration _configuration;
    private readonly PhotoCarouselDbContext _dbContext;
    private readonly ILogger<FolderService> _logger;

    public FolderService(
        IConfiguration configuration,
        PhotoCarouselDbContext dbContext,
        ILogger<FolderService> logger)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<FolderContract>> GetFolders()
    {
        var allFolders = await _dbContext.Photos.Select(x => x.FolderPath).Distinct().ToListAsync();

        if (allFolders.Count == 0)
        {
            return null;
        }

        var foldersToReturn = new List<FolderContract>();

        foreach (var folderPath in allFolders)
        {
            ParseFolders(folderPath, foldersToReturn);
        }

        return foldersToReturn;
    }

    private void ParseFolders(string folderPath, List<FolderContract> foldersToReturn)
    {
        var folderPathParts = folderPath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

        var currentFolders = foldersToReturn;
        var fullPathBuilder = new StringBuilder();

        foreach (var folderPathPart in folderPathParts)
        {
            fullPathBuilder.Append("/");
            fullPathBuilder.Append(folderPathPart);

            var existingFolderPart = currentFolders.SingleOrDefault(x => x.Name == folderPathPart);
            if (existingFolderPart == null)
            {
                existingFolderPart = new FolderContract
                {
                    Name = folderPathPart,
                    FullPath = fullPathBuilder.ToString(),
                    ChildFolders = new List<FolderContract>()
                };

                currentFolders.Add(existingFolderPart);
            }

            currentFolders = existingFolderPart.ChildFolders;
        }
    }
}