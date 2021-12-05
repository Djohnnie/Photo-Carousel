using Avalonia.Media.Imaging;
using PhotoCarousel.Display.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;

namespace PhotoCarousel.Display.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private HttpClient _httpClient;

        public string Greeting => "Welcome to Avalonia!";

        public IBitmap TestImage { get; set; }

        public string TestDescription { get; set; }

        public MainWindowViewModel()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://192.168.10.2:8077");

            var response = _httpClient.GetFromJsonAsync<Photo>("photos/random").Result;
            var photo = _httpClient.GetByteArrayAsync($"downloads/photo/{response.Id}").Result;

            using var stream = new MemoryStream(photo);
            TestImage = new Bitmap(stream);
            TestDescription = response.Description;
        }
    }
}