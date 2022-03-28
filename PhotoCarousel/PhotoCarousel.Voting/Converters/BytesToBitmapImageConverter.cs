using System.Globalization;

namespace PhotoCarousel.Voting.Converters
{
    public class BytesToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageBytes = value as byte[];

            ImageSource image = null;

            if (imageBytes != null)
            {
                image = ImageSource.FromStream(() =>
                {
                    var imageStream = new MemoryStream(imageBytes);
                    imageStream.Position = 0;
                    return imageStream;
                });
            }

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}