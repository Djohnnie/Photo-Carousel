using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Contracts;
using PhotoCarousel.DataAccess;

namespace PhotoCarousel.Api.Services;

public class RatingService
{
    private readonly IConfiguration _configuration;
    private readonly PhotoCarouselDbContext _dbContext;
    private readonly ILogger<RatingService> _logger;

    public RatingService(
        IConfiguration configuration,
        PhotoCarouselDbContext dbContext,
        ILogger<RatingService> logger)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SetRating(PhotoRating photoRating)
    {
        var allPhotos = await _dbContext.Photos.Where(
            x => photoRating.PhotoIds.Contains(x.Id)).ToListAsync();

        allPhotos.ForEach(photo => photo.Rating = photoRating.Rating);

        await _dbContext.SaveChangesAsync();
    }
}