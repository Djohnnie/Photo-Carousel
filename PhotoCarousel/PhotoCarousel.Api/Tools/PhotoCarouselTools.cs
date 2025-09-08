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

    [McpServerTool(Name = $"photocarousel_{nameof(GetCurrentPhotoInformation)}", ReadOnly = true)]
    [Description("Gets information about the current displayed photo.")]
    [return: Description("Information, formatted in JSON containing a short description that should be used if the info is empty and a date that the photo is taken.")]
    public async Task<string> GetCurrentPhotoInformation()
    {
        var currentPhoto = await _photoService.GetCurrentPhoto();
        return await GetPhotoDescription(currentPhoto.Id);
    }

    [McpServerTool(Name = $"photocarousel_{nameof(GetPreviousPhotoInformation)}", ReadOnly = true)]
    [Description("Gets information about the previously displayed photo.")]
    [return: Description("Information, formatted in JSON containing a short description that should be used if the info is empty and a date that the photo is taken.")]
    public async Task<string> GetPreviousPhotoInformation()
    {
        var previousPhoto = await _photoService.GetPreviousPhoto();
        return await GetPhotoDescription(previousPhoto.Id);
    }

    [McpServerTool(Name = $"photocarousel_{nameof(GetNextPhotoInformation)}", ReadOnly = true)]
    [Description("Gets information about the photo that will be shown next.")]
    [return: Description("Information, formatted in JSON containing a short description that should be used if the info is empty and a date that the photo is taken.")]
    public async Task<string> GetNextPhotoInformation()
    {
        var nextPhoto = await _photoService.GetPreviousPhoto();
        return await GetPhotoDescription(nextPhoto.Id);
    }

    private async Task<string> GetPhotoDescription(Guid photoId)
    {
        var photo = await _photoService.GetPhotoById(photoId);
        var (description, dateTaken) = SplitDescription(photo.Description);
        return JsonSerializer.Serialize(new PhotoInfo
        {
            Description = description,
            DateTaken = dateTaken,
            Info = photo.Info
        });
    }

    private (string, string) SplitDescription(string description)
    {
        try
        {
            var parts = description.Split([' '], StringSplitOptions.RemoveEmptyEntries);

            var datePart = parts[0];
            var descriptionPart = string.Join(' ', parts[1..]);

            return ($"{descriptionPart.Replace("(", "").Replace(")", "")}", datePart);
        }
        catch
        {
            return (description, string.Empty);
        }
    }

    private class PhotoInfo
    {
        public string Description { get; set; }
        public string DateTaken { get; set; }
        public string Info { get; set; }
    }
}