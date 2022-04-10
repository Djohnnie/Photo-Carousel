using PhotoCarousel.Enums;
using PhotoCarousel.Voting.Helpers;
using PhotoCarousel.Voting.ViewModels.Base;
using System.Windows.Input;

namespace PhotoCarousel.Voting.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ApiClientHelper _apiClientHelper;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));

    private string _errorDescription;
    public string ErrorDescription
    {
        get => _errorDescription;
        set
        {
            _errorDescription = value;
            OnPropertyChanged(nameof(ErrorDescription));
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
            OnPropertyChanged(nameof(PreviousPhoto));
        }
    }

    private Rating _previousPhotoRating;
    public Rating PreviousPhotoRating
    {
        get => _previousPhotoRating;
        set
        {
            _previousPhotoRating = value;
            OnPropertyChanged(nameof(PreviousPhotoRating));
        }
    }

    private string _previousPhotoDescription;
    public string PreviousPhotoDescription
    {
        get => _previousPhotoDescription;
        set
        {
            _previousPhotoDescription = value;
            OnPropertyChanged(nameof(PreviousPhotoDescription));
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
            OnPropertyChanged(nameof(CurrentPhoto));
        }
    }

    private Rating _currentPhotoRating;
    public Rating CurrentPhotoRating
    {
        get => _currentPhotoRating;
        set
        {
            _currentPhotoRating = value;
            OnPropertyChanged(nameof(CurrentPhotoRating));
        }
    }

    private string _currentPhotoDescription;
    public string CurrentPhotoDescription
    {
        get => _currentPhotoDescription;
        set
        {
            _currentPhotoDescription = value;
            OnPropertyChanged(nameof(CurrentPhotoDescription));
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
            OnPropertyChanged(nameof(NextPhoto));
        }
    }

    private Rating _nextPhotoRating;
    public Rating NextPhotoRating
    {
        get => _nextPhotoRating;
        set
        {
            _nextPhotoRating = value;
            OnPropertyChanged(nameof(NextPhotoRating));
        }
    }

    private string _nextPhotoDescription;
    public string NextPhotoDescription
    {
        get => _nextPhotoDescription;
        set
        {
            _nextPhotoDescription = value;
            OnPropertyChanged(nameof(NextPhotoDescription));
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

    public MainViewModel(ApiClientHelper apiClientHelper)
    {
        _apiClientHelper = apiClientHelper;

        PreviousPhotoThumbsUpCommand = new Command(OnPreviousPhotoThumbsUp);
        PreviousPhotoResetCommand = new Command(OnPreviousPhotoReset);
        PreviousPhotoThumbsDownCommand = new Command(OnPreviousPhotoThumbsDown);
        CurrentPhotoThumbsUpCommand = new Command(OnCurrentPhotoThumbsUp);
        CurrentPhotoResetCommand = new Command(OnCurrentPhotoReset);
        CurrentPhotoThumbsDownCommand = new Command(OnCurrentPhotoThumbsDown);
        NextPhotoThumbsUpCommand = new Command(OnNextPhotoThumbsUp);
        NextPhotoResetCommand = new Command(OnNextPhotoReset);
        NextPhotoThumbsDownCommand = new Command(OnNextPhotoThumbsDown);

        Task.Run(async () =>
        {
            do
            {
                try
                {
                    await RefreshScheduledPhotos();
                }
                catch (Exception ex)
                {
                    ErrorDescription = ex.Message;
                }
            }
            while (await _timer.WaitForNextTickAsync());
        });
    }

    private async void OnPreviousPhotoThumbsUp(object args)
    {
        await SetRating(_previousPhotoId, Rating.ThumbsUp);
        await RefreshScheduledPhotos();
    }

    private async void OnPreviousPhotoReset(object args)
    {
        await SetRating(_previousPhotoId, Rating.None);
        await RefreshScheduledPhotos();
    }

    private async void OnPreviousPhotoThumbsDown(object args)
    {
        await SetRating(_previousPhotoId, Rating.ThumbsDown);
        await RefreshScheduledPhotos();
    }

    private async void OnCurrentPhotoThumbsUp(object args)
    {
        await SetRating(_currentPhotoId, Rating.ThumbsUp);
        await RefreshScheduledPhotos();
    }

    private async void OnCurrentPhotoReset(object args)
    {
        await SetRating(_currentPhotoId, Rating.None);
        await RefreshScheduledPhotos();
    }

    private async void OnCurrentPhotoThumbsDown(object args)
    {
        await SetRating(_currentPhotoId, Rating.ThumbsDown);
        await RefreshScheduledPhotos();
    }

    private async void OnNextPhotoThumbsUp(object args)
    {
        await SetRating(_nextPhotoId, Rating.ThumbsUp);
        await RefreshScheduledPhotos();
    }

    private async void OnNextPhotoReset(object args)
    {
        await SetRating(_nextPhotoId, Rating.None);
        await RefreshScheduledPhotos();
    }

    private async void OnNextPhotoThumbsDown(object args)
    {
        await SetRating(_nextPhotoId, Rating.ThumbsDown);
        await RefreshScheduledPhotos();
    }

    private async Task SetRating(Guid photoId, Rating rating)
    {
        try
        {
            await _apiClientHelper.SetRating(new[] { photoId }, rating);
        }
        catch
        {
            // Nothing we can do...
        }
    }

    private async Task RefreshScheduledPhotos()
    {
        var previousPhoto = await _apiClientHelper.GetPreviousPhoto();
        if (previousPhoto.Id != Guid.Empty)
        {
            _previousPhotoId = previousPhoto.Id;
            PreviousPhoto = await _apiClientHelper.GetThumbnail(previousPhoto.Id);
            PreviousPhotoRating = previousPhoto.Rating;
            PreviousPhotoDescription = previousPhoto.Description;
        }
        else
        {
            PreviousPhotoRating = Rating.None;
            PreviousPhoto = null;
            PreviousPhotoDescription = string.Empty;
        }

        var currentPhoto = await _apiClientHelper.GetCurrentPhoto();
        if (currentPhoto.Id != Guid.Empty)
        {
            _currentPhotoId = currentPhoto.Id;
            CurrentPhoto = await _apiClientHelper.GetThumbnail(currentPhoto.Id);
            CurrentPhotoRating = currentPhoto.Rating;
            CurrentPhotoDescription = currentPhoto.Description;
        }
        else
        {
            CurrentPhotoRating = Rating.None;
            CurrentPhoto = null;
            CurrentPhotoDescription = string.Empty;
        }

        var nextPhoto = await _apiClientHelper.GetNextPhoto();
        if (nextPhoto.Id != Guid.Empty)
        {
            _nextPhotoId = nextPhoto.Id;
            NextPhoto = await _apiClientHelper.GetThumbnail(nextPhoto.Id);
            NextPhotoRating = nextPhoto.Rating;
            NextPhotoDescription = nextPhoto.Description;
        }
        else
        {
            NextPhotoRating = Rating.None;
            NextPhoto = null;
            NextPhotoDescription = string.Empty;
        }
    }
}