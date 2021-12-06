using Avalonia.Media.Imaging;
using PhotoCarousel.Display.Models;
using ReactiveUI;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PhotoCarousel.Display.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly SynchronizationContext _synchronizationContext = SynchronizationContext.Current;
        private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        private HttpClient _httpClient;

        public string Greeting => "Welcome to Avalonia!";

        private IBitmap _testImage;
        public IBitmap TestImage
        {
            get
            {
                return _testImage;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _testImage, value);
            }
        }

        private string _testDescription;
        public string TestDescription
        {
            get
            {
                return _testDescription;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _testDescription, value);
            }
        }

        public MainWindowViewModel()
        {
            _ = Task.Run(async () =>
            {
                do
                {
                    try
                    {
                        _httpClient = new HttpClient();
                        _httpClient.BaseAddress = new Uri("http://192.168.10.2:8077");

                        var response = _httpClient.GetFromJsonAsync<Photo>("photos/random").Result;
                        var photo = _httpClient.GetByteArrayAsync($"downloads/photo/{response.Id}").Result;

                        _synchronizationContext.Post((x) =>
                        {
                            using var stream = new MemoryStream(photo);
                            TestImage = new Bitmap(stream);
                            TestDescription = response.Description;
                        }, null);
                    }
                    catch { }
                } while (await _timer.WaitForNextTickAsync());
            });
        }
    }
}