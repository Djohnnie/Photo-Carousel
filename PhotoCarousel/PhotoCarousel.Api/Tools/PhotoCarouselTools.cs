using ModelContextProtocol.Server;
using PhotoCarousel.Api.Services;
using System;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace PhotoCarousel.Api.Tools;

[McpServerToolType]
public class PhotoCarouselTools
{
    private readonly PhotoService _photoService;

    public PhotoCarouselTools(PhotoService photoService)
    {
        _photoService = photoService;
    }

    [McpServerTool]
    [Description("Gets information about the photo with given id.")]
    public async Task<string> GetPhotoInformation(
        [Description("The id of the photo to get the information for.")]
        Guid photoId)
    {
        var photo = await _photoService.GetPhotoById(photoId);
        return JsonSerializer.Serialize(photo);
    }
}