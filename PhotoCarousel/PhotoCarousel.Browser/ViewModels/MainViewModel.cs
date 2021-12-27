using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PhotoCarousel.Browser.Helpers;
using PhotoCarousel.Browser.Models;
using PhotoCarousel.Browser.Wpf;
using PhotoCarousel.Contracts;
using PhotoCarousel.Enums;

namespace PhotoCarousel.Browser.ViewModels;

internal class MainViewModel : ViewModelBase
{
    protected readonly ApiClientHelper _apiClientHelper;

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

    public ICommand ThumbsUpAllCommand { get; set; }
    public ICommand ThumbsUpSelectedCommand { get; set; }

    public ICommand ResetAllCommand { get; set; }
    public ICommand ResetSelectedCommand { get; set; }

    public ICommand ThumbsDownAllCommand { get; set; }
    public ICommand ThumbsDownSelectedCommand { get; set; }

    public MainViewModel()
    {
        _apiClientHelper = new ApiClientHelper();

        ThumbsUpAllCommand = new RelayCommand(OnThumbsUpAll);
        ThumbsUpSelectedCommand = new RelayCommand(OnThumbsUpSelected);
        ResetAllCommand = new RelayCommand(OnResetAll);
        ResetSelectedCommand = new RelayCommand(OnResetSelected);
        ThumbsDownAllCommand = new RelayCommand(OnThumbsDownAll);
        ThumbsDownSelectedCommand = new RelayCommand(OnThumbsDownSelected);

        Task.Run(async () =>
        {
            Folders = await _apiClientHelper.GetFolders();
        });
    }

    private async void OnThumbsUpAll(object args)
    {
        await SetRating(GetAllPhotoIds(), Rating.ThumbsUp);
    }

    private async void OnThumbsUpSelected(object args)
    {
        await SetRating(GetSelectedPhotoIds(), Rating.ThumbsUp);
    }

    private async void OnResetAll(object args)
    {
        await SetRating(GetAllPhotoIds(), Rating.None);
    }

    private async void OnResetSelected(object args)
    {
        await SetRating(GetSelectedPhotoIds(), Rating.None);
    }

    private async void OnThumbsDownAll(object args)
    {
        await SetRating(GetAllPhotoIds(), Rating.ThumbsDown);
    }

    private async void OnThumbsDownSelected(object args)
    {
        await SetRating(GetSelectedPhotoIds(), Rating.ThumbsDown);
    }

    private Guid[] GetAllPhotoIds()
    {
        return Photos.Select(x => x.Id).ToArray();
    }

    private Guid[] GetSelectedPhotoIds()
    {
        return Photos.Where(x => x.IsSelected).Select(x => x.Id).ToArray();
    }

    private async Task SetRating(Guid[] photoIds, Rating rating)
    {
        await _apiClientHelper.SetRating(photoIds, rating);
        await RefreshPhotos();
    }

    private async Task RefreshPhotos()
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
                Name = x.Description,
                Rating = x.Rating
            }).ToList();

            await Parallel.ForEachAsync(Photos, async (photo, _) =>
            {
                photo.Bitmap = await _apiClientHelper.GetThumbnail(photo.Id);
            });
        }

        GC.Collect();
    }
}