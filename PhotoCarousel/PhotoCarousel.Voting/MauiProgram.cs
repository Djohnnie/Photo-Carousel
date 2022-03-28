using PhotoCarousel.Voting.Helpers;
using PhotoCarousel.Voting.ViewModels;
using PhotoCarousel.Voting.Views;

namespace PhotoCarousel.Voting
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("fa6-free-solid-900.otf", "FontAwesomeSolid");
                });
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<ApiClientHelper>();

            return builder.Build();
        }
    }
}