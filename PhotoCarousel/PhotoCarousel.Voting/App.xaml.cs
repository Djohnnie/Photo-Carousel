using PhotoCarousel.Voting.Views;

namespace PhotoCarousel.Voting
{
    public partial class App : Application
    {
        public App(MainPage mainPage)
        {
            InitializeComponent();

            MainPage = mainPage;
        }
    }
}