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
    [Description("Gets the id for the photo currently shown.")]
    public async Task<Guid> GetCurrentPhotoId()
    {
        var photo = await _photoService.GetCurrentPhoto();
        return photo.Id;
    }

    [McpServerTool]
    [Description("Gets information about the photo with given id.")]
    [return: Description("Information, formatted in JSON containing a short description that should be used if the info is empty and a date that the photo is taken.")]
    public async Task<string> GetPhotoInformation(
        [Description("The id of the photo to get the information for.")]
        Guid photoId)
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
            var parts = description.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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