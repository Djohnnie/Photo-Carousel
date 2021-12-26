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
        var photoIds = Photos.Select(x => x.Id).ToArray();
        await _apiClientHelper.SetRating(photoIds, Rating.ThumbsUp);
        await RefreshPhotos();
    }

    private async void OnThumbsUpSelected(object args)
    {
        throw new NotImplementedException();
    }

    private async void OnResetAll(object args)
    {
        var photoIds = Photos.Select(x => x.Id).ToArray();
        await _apiClientHelper.SetRating(photoIds, Rating.None);
        await RefreshPhotos();
    }

    private async void OnResetSelected(object args)
    {
        throw new NotImplementedException();
    }

    private async void OnThumbsDownAll(object args)
    {
        var photoIds = Photos.Select(x => x.Id).ToArray();
        await _apiClientHelper.SetRating(photoIds, Rating.ThumbsDown);
        await RefreshPhotos();
    }

    private async void OnThumbsDownSelected(object args)
    {
        throw new NotImplementedException();
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