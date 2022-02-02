﻿using System.Reflection;
using System.Windows;
using PhotoCarousel.Browser.ViewModels;

namespace PhotoCarousel.Browser.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}