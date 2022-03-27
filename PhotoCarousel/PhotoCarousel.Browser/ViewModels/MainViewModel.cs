using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));

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

    private Guid _previousPhotoId;
    private byte[] _previousPhoto;

    public byte[] PreviousPhoto
    {
        get => _previousPhoto;
        set
        {
            _previousPhoto = value;
            OnPropertyChanged();
        }
    }

    private Rating _previousPhotoRating;

    public Rating PreviousPhotoRating
    {
        get => _previousPhotoRating;
        set
        {
            _previousPhotoRating = value;
            OnPropertyChanged();
        }
    }

    private Guid _currentPhotoId;
    private byte[] _currentPhoto;

    public byte[] CurrentPhoto
    {
        get => _currentPhoto;
        set
        {
            _currentPhoto = value;
            OnPropertyChanged();
        }
    }

    private Rating _currentPhotoRating;

    public Rating CurrentPhotoRating
    {
        get => _currentPhotoRating;
        set
        {
            _currentPhotoRating = value;
            OnPropertyChanged();
        }
    }

    private Guid _nextPhotoId;
    private byte[] _nextPhoto;

    public byte[] NextPhoto
    {
        get => _nextPhoto;
        set
        {
            _nextPhoto = value;
            OnPropertyChanged();
        }
    }

    private Rating _nextPhotoRating;

    public Rating NextPhotoRating
    {
        get => _nextPhotoRating;
        set
        {
            _nextPhotoRating = value;
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

    public ICommand PreviousPhotoThumbsUpCommand { get; set; }
    public ICommand PreviousPhotoResetCommand { get; set; }
    public ICommand PreviousPhotoThumbsDownCommand { get; set; }

    public ICommand CurrentPhotoThumbsUpCommand { get; set; }
    public ICommand CurrentPhotoResetCommand { get; set; }
    public ICommand CurrentPhotoThumbsDownCommand { get; set; }

    public ICommand NextPhotoThumbsUpCommand { get; set; }
    public ICommand NextPhotoResetCommand { get; set; }
    public ICommand NextPhotoThumbsDownCommand { get; set; }

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

        PreviousPhotoThumbsUpCommand = new RelayCommand(OnPreviousPhotoThumbsUp);
        PreviousPhotoResetCommand = new RelayCommand(OnPreviousPhotoReset);
        PreviousPhotoThumbsDownCommand = new RelayCommand(OnPreviousPhotoThumbsDown);
        CurrentPhotoThumbsUpCommand = new RelayCommand(OnCurrentPhotoThumbsUp);
        CurrentPhotoResetCommand = new RelayCommand(OnCurrentPhotoReset);
        CurrentPhotoThumbsDownCommand = new RelayCommand(OnCurrentPhotoThumbsDown);
        NextPhotoThumbsUpCommand = new RelayCommand(OnNextPhotoThumbsUp);
        NextPhotoResetCommand = new RelayCommand(OnNextPhotoReset);
        NextPhotoThumbsDownCommand = new RelayCommand(OnNextPhotoThumbsDown);

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

            do
            {
                await RefreshScheduledPhotos();
            } while (await _timer.WaitForNextTickAsync());
        });
    }

    private async void OnPreviousPhotoThumbsUp(object args)
    {
        await SetRating(new[] { _previousPhotoId }, Rating.ThumbsUp);
        await RefreshScheduledPhotos();
    }

    private async void OnPreviousPhotoReset(object args)
    {
        await SetRating(new[] { _previousPhotoId }, Rating.None);
        await RefreshScheduledPhotos();
    }

    private async void OnPreviousPhotoThumbsDown(object args)
    {
        await SetRating(new[] { _previousPhotoId }, Rating.ThumbsDown);
        await RefreshScheduledPhotos();
    }

    private async void OnCurrentPhotoThumbsUp(object args)
    {
        await SetRating(new[] { _currentPhotoId }, Rating.ThumbsUp);
        await RefreshScheduledPhotos();
    }

    private async void OnCurrentPhotoReset(object args)
    {
        await SetRating(new[] { _currentPhotoId }, Rating.None);
        await RefreshScheduledPhotos();
    }

    private async void OnCurrentPhotoThumbsDown(object args)
    {
        await SetRating(new[] { _currentPhotoId }, Rating.ThumbsDown);
        await RefreshScheduledPhotos();
    }

    private async void OnNextPhotoThumbsUp(object args)
    {
        await SetRating(new[] { _nextPhotoId }, Rating.ThumbsUp);
        await RefreshScheduledPhotos();
    }

    private async void OnNextPhotoReset(object args)
    {
        await SetRating(new[] { _nextPhotoId }, Rating.None);
        await RefreshScheduledPhotos();
    }

    private async void OnNextPhotoThumbsDown(object args)
    {
        await SetRating(new[] { _nextPhotoId }, Rating.ThumbsDown);
        await RefreshScheduledPhotos();
    }

    private async void OnThumbsUpAll(object args)
    {
        await SetRating(GetAllPhotoIds(), Rating.ThumbsUp);
        await RefreshPhotos();
    }

    private async void OnThumbsUpSelected(object args)
    {
        await SetRating(GetSelectedPhotoIds(), Rating.ThumbsUp);
        await RefreshPhotos();
    }

    private async void OnResetAll(object args)
    {
        await SetRating(GetAllPhotoIds(), Rating.None);
        await RefreshPhotos();
    }

    private async void OnResetSelected(object args)
    {
        await SetRating(GetSelectedPhotoIds(), Rating.None);
        await RefreshPhotos();
    }

    private async void OnThumbsDownAll(object args)
    {
        await SetRating(GetAllPhotoIds(), Rating.ThumbsDown);
        await RefreshPhotos();
    }

    private async void OnThumbsDownSelected(object args)
    {
        await SetRating(GetSelectedPhotoIds(), Rating.ThumbsDown);
        await RefreshPhotos();
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
        try
        {
            await _apiClientHelper.SetRating(photoIds, rating);
        }
        catch
        {
            // Nothing we can do...
        }
    }

    private async Task RefreshScheduledPhotos()
    {
        try
        {
            var previousPhoto = await _apiClientHelper.GetPreviousPhoto();
            _previousPhotoId = previousPhoto.Id;
            PreviousPhoto = await _apiClientHelper.GetThumbnail(previousPhoto.Id);
            PreviousPhotoRating = previousPhoto.Rating;

            var currentPhoto = await _apiClientHelper.GetCurrentPhoto();
            _currentPhotoId = currentPhoto.Id;
            CurrentPhoto = await _apiClientHelper.GetThumbnail(currentPhoto.Id);
            CurrentPhotoRating = currentPhoto.Rating;

            var nextPhoto = await _apiClientHelper.GetNextPhoto();
            _nextPhotoId = nextPhoto.Id;
            NextPhoto = await _apiClientHelper.GetThumbnail(nextPhoto.Id);
            NextPhotoRating = nextPhoto.Rating;
        }
        catch
        {
            // Nothing we can do...
        }
    }

    private async Task RefreshPhotos()
    {
        try
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
        catch
        {
            // Nothing we can do...
        }
    }
}