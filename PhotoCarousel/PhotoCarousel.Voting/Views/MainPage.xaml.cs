using PhotoCarousel.Voting.ViewModels;

namespace PhotoCarousel.Voting.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}