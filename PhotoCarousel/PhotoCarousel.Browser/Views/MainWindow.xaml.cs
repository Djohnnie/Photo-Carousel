using PhotoCarousel.Browser.ViewModels;
using System.Windows;

namespace PhotoCarousel.Browser.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}