using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using PhotoCarousel.Enums;

namespace PhotoCarousel.Browser.Converters;

internal class RatingToPackIconKindConverter : IValueConverter
{
    public PackIconKind Neutral { get; set; }
    public PackIconKind ThumbsDown { get; set; }
    public PackIconKind ThumbsUp { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var rating = value as Rating?;

        if (rating != null && rating == Rating.ThumbsDown)
        {
            return ThumbsDown;
        }

        if (rating != null && rating == Rating.ThumbsUp)
        {
            return ThumbsUp;
        }

        return Neutral;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}