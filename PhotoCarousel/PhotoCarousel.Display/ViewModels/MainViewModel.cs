using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Net.Http.Json;
using System.IO;
using PhotoCarousel.Contracts;
using System.Text;

namespace PhotoCarousel.Display.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(1));

    private Bitmap _testImage;
    public Bitmap TestImage
    {
        get => _testImage;
        set => this.RaiseAndSetIfChanged(ref _testImage, value);
    }

    private string _testDescription;
    public string TestDescription
    {
        get => _testDescription;
        set => this.RaiseAndSetIfChanged(ref _testDescription, value);
    }

    private string _errorDescription;
    public string ErrorDescription
    {
        get => _errorDescription;
        set => this.RaiseAndSetIfChanged(ref _errorDescription, value);
    }

    public MainViewModel()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://api.djohnnie.be:8077");

        _ = Task.Run(async () =>
        {
            do
            {
                var error = false;
                do
                {
                    try
                    {
                        var response = await httpClient.GetFromJsonAsync<Photo>("photos/current");
                        var photo = await httpClient.GetByteArrayAsync($"downloads/photo/{response.Id}");

                        var flag = new DisplayPingFlag
                        {
                            LastPing = DateTime.Now
                        };

                        await httpClient.PostAsJsonAsync("flags/set", new Flag
                        {
                            Name = DisplayPingFlag.Name,
                            Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(flag.Serialize()))
                        });

                        if (_synchronizationContext != null)
                        {
                            _synchronizationContext.Post((x) =>
                            {
                                using var stream = new MemoryStream(photo);
                                TestImage = new Bitmap(stream);
                                TestDescription = ConvertDescription(response.Description);
                                ErrorDescription = string.Empty;
                            }, null);
                        }

                        error = false;
                    }
                    catch (Exception ex)
                    {
                        if (_synchronizationContext != null)
                        {
                            _synchronizationContext.Post((x) =>
                            {
                                ErrorDescription = ex.Message;
                            }, null);
                        }

                        error = true;
                        await Task.Delay(1000);
                    }
                } while (error);
            } while (await _timer.WaitForNextTickAsync());
        });
    }

    private string ConvertDescription(string description)
    {
        try
        {
            var parts = description.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var datePart = parts[0];
            var descriptionPart = string.Join(' ', parts[1..]);

            return $"{descriptionPart.Replace("(", "").Replace(")", "")} ({datePart})";
        }
        catch
        {
            return description;
        }
    }
}