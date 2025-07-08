using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PhotoCarousel.Contracts;
using PhotoCarousel.DataAccess;
using PhotoCarousel.Entities;
using System;
using System.Buffers.Text;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Services;

public class FlagsService
{
    private readonly IConfiguration _configuration;
    private readonly PhotoCarouselDbContext _dbContext;
    private readonly ILogger<FlagsService> _logger;

    public FlagsService(
        IConfiguration configuration,
        PhotoCarouselDbContext dbContext,
        ILogger<FlagsService> logger)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IFlag> GetFlag(string name)
    {
        var flag = await _dbContext.Flags
            .FirstOrDefaultAsync(f => f.Name == name);

        switch (name)
        {
            case DisplayPingFlag.Name:
                return flag is null ? DisplayPingFlag.Default : DisplayPingFlag.Deserialize(flag.Value);
            default:
                _logger.LogWarning($"Unknown flag requested: {name}");
                return null;
        }
    }

    public async Task SetFlag(string name, string value)
    {
        var binary = Convert.FromBase64String(value);
        var decodedValue = Encoding.UTF8.GetString(binary);

        var existingFlag = await _dbContext.Flags
            .FirstOrDefaultAsync(f => f.Name == name);

        if (existingFlag is null)
        {
            _dbContext.Flags.Add(new Entities.Flag
            {
                Id = Guid.NewGuid(),
                Name = name,
                Value = decodedValue,
            });
        }
        else
        {
            existingFlag.Value = decodedValue;
        }

        await _dbContext.SaveChangesAsync();
    }
}