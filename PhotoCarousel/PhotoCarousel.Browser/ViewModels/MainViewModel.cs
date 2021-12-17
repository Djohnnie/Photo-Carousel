using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PhotoCarousel.Browser.Annotations;
using PhotoCarousel.Browser.Helpers;
using PhotoCarousel.Browser.Models;
using PhotoCarousel.Contracts;

namespace PhotoCarousel.Browser.ViewModels;

internal class MainViewModel : INotifyPropertyChanged
{
    private readonly ApiClientHelper _apiClientHelper;

    private Folder _selectedFolder;

    public Folder SelectedFolder
    {
        get { return _selectedFolder; }
        set
        {
            _selectedFolder = value;
            OnPropertyChanged();
        }
    }

    private List<Folder> _folders;

    public List<Folder> Folders
    {
        get { return _folders; }
        set
        {
            _folders = value;
            OnPropertyChanged();
        }
    }

    private List<PhotoItem> _photos;

    public List<PhotoItem> Photos
    {
        get { return _photos; }
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
            var photos = await _apiClientHelper.GetPhotos("/photo/2014/2014-09 (New York en Canada)/2014-09-21");
            Photos = photos.Select(x => new PhotoItem
            {
                Id = x.Id,
                Name = x.Description
            }).ToList();

            await Parallel.ForEachAsync(Photos, async (photo, _) =>
            {
                photo.Bitmap = await _apiClientHelper.GetThumbnail(photo.Id);
            });
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}