using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhotoCarousel.Browser.Helpers;
using PhotoCarousel.Browser.Models;
using PhotoCarousel.Contracts;

namespace PhotoCarousel.Browser.ViewModels;

internal class MainViewModel : ViewModelBase
{
    private readonly ApiClientHelper _apiClientHelper;

    private Folder _selectedFolder;

    public Folder SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            _selectedFolder = value;
            _ = Task.Run(async () => await RefreshPhotos());
            OnPropertyChanged();
        }
    }

    private List<Folder> _folders;

    public List<Folder> Folders
    {
        get => _folders;
        set
        {
            _folders = value;
            OnPropertyChanged();
        }
    }

    private List<PhotoItem> _photos;

    public List<PhotoItem> Photos
    {
        get => _photos;
        set
        {
            _photos = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel()
    {
        _apiClientHelper = new ApiClientHelper();

        Task.Run(async () =>
        {
            Folders = await _apiClientHelper.GetFolders();
        });
    }

    public async Task RefreshPhotos()
    {
        if (SelectedFolder == null)
        {
            Photos = new List<PhotoItem>();
        }
        else
        {
            var photos = await _apiClientHelper.GetPhotos(SelectedFolder.FullPath);
            Photos = photos.Select(x => new PhotoItem
            {
                Id = x.Id,
                Name = x.Description
            }).ToList();

            await Parallel.ForEachAsync(Photos, async (photo, _) =>
            {
                photo.Bitmap = await _apiClientHelper.GetThumbnail(photo.Id);
                await Task.Delay(100);
            });
        }
    }
}