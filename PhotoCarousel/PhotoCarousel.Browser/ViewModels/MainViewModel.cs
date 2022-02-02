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
    protected readonly UpdateHelper _updateHelper;

    private string _title;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    private bool _updateAvailable;

    public bool UpdateAvailable
    {
        get => _updateAvailable;
        set
        {
            _updateAvailable = value;
            OnPropertyChanged();
        }
    }

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

    public ICommand UpdateCommand { get; set; }

    public MainViewModel()
    {
        _apiClientHelper = new ApiClientHelper();
        _updateHelper = new UpdateHelper();

        ThumbsUpAllCommand = new RelayCommand(OnThumbsUpAll);
        ThumbsUpSelectedCommand = new RelayCommand(OnThumbsUpSelected);
        ResetAllCommand = new RelayCommand(OnResetAll);
        ResetSelectedCommand = new RelayCommand(OnResetSelected);
        ThumbsDownAllCommand = new RelayCommand(OnThumbsDownAll);
        ThumbsDownSelectedCommand = new RelayCommand(OnThumbsDownSelected);
        UpdateCommand = new RelayCommand(OnUpdate);

        Task.Run(async () =>
        {
            var currentVersion = _updateHelper.GetCurrentVersion();
            var availableVersion = await _updateHelper.GetAvailableVersion();
            Title = $"PhotoCarousel Browser ({currentVersion})";

            Folders = await _apiClientHelper.GetFolders();

            UpdateAvailable = _updateHelper.IsAvailableNewer(currentVersion, availableVersion);
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

    private async void OnUpdate(object args)
    {
        var availableVersion = await _updateHelper.GetAvailableVersion();
        var updatePath = await _updateHelper.DownloadAvailableVersion(availableVersion);
        _updateHelper.InstallUpdate(updatePath);
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