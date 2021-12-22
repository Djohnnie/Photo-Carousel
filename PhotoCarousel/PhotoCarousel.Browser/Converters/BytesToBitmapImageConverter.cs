using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PhotoCarousel.Browser.Converters;

internal class BytesToBitmapImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var imageBytes = value as byte[];

        BitmapImage image = new BitmapImage();

        if (imageBytes != null)
        {
            using MemoryStream memStream = new MemoryStream(imageBytes);
            image.DecodePixelWidth = 150;
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = memStream;
            image.EndInit();
            image.Freeze();
        }

        return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}