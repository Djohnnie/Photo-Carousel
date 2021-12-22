using System.Linq;
using System.Threading.Tasks;
using PhotoCarousel.Browser.Models;

namespace PhotoCarousel.Browser.ViewModels.Design;

internal class MainDesignViewModel : MainViewModel
{
    public MainDesignViewModel() : base()
    {
        Task.Run(async () =>
        {
            var photos = await _apiClientHelper.GetPhotos("/photo/2014/2014-10 (Schilderen Willebroek)");
            Photos = photos.Select(x => new PhotoItem
            {
                Id = x.Id,
                Name = x.Description,
                Rating = x.Rating
            }).ToList();

            await Parallel.ForEachAsync(Photos, async (photo, _) =>
            {
                photo.Bitmap = await _apiClientHelper.GetThumbnail(photo.Id);
                await Task.Delay(100);
            });
        });
    }
}