using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Contracts;
using PhotoCarousel.DataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Services;

public class DuplicatesService
{
    private readonly IConfiguration _configuration;
    private readonly PhotoCarouselDbContext _dbContext;
    private readonly ILogger<DuplicatesService> _logger;

    public DuplicatesService(
        IConfiguration configuration,
        PhotoCarouselDbContext dbContext,
        ILogger<DuplicatesService> logger)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<Duplicates>> GetDuplicates()
    {
        var duplicateHashes = await _dbContext.Photos
            .GroupBy(x => x.Sha256Hash)
            .Where(x => x.Count() > 1)
            .OrderByDescending(x => x.Count())
            .Select(x => x.Key)
            .ToListAsync();

        var photos = await _dbContext.Photos
            .Where(x => duplicateHashes.Contains(x.Sha256Hash))
            .ToListAsync();

        var duplicates = new List<Duplicates>();

        foreach (var duplicateHash in duplicateHashes)
        {
            var duplicate = new Duplicates
            {
                Sha256Hash = duplicateHash,
                Photos = new List<DuplicatePhoto>()
            };

            foreach (var photo in photos.Where(x => x.Sha256Hash == duplicateHash))
            {
                duplicate.Photos.Add(new DuplicatePhoto
                {
                    Id = photo.Id,
                    Description = photo.Description,
                    SourcePath = photo.SourcePath
                });
            }

            duplicates.Add(duplicate);
        }

        return duplicates;
    }
}