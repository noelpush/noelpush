    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

namespace NPush.Converters
{
    public class BitmapToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bitmap = value as Bitmap;

            if (bitmap == null)
                return null;

            return new ImageSourceConverter().ConvertFrom(bitmap);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
