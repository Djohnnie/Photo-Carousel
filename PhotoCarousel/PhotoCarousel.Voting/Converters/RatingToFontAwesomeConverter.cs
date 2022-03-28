using PhotoCarousel.Enums;
using System.Globalization;

namespace PhotoCarousel.Voting.Converters
{
    public class RatingToFontAwesomeConverter : IValueConverter
    {
        public string Neutral { get; set; }
        public string ThumbsDown { get; set; }
        public string ThumbsUp { get; set; }

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
}