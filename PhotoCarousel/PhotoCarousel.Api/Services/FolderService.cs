using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        return allFolders.Select(x => new FolderContract
        {
            Name = new DirectoryInfo(x).Name,
            FullPath = x
        }).ToList();
    }
}