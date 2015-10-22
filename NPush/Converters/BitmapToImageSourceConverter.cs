﻿    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

namespace NPush.Converters
{
    public class BitmapToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bitmap = value as Bitmap;

            if (bitmap == null)
                return null;

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                      bitmap.GetHbitmap(),
                      IntPtr.Zero,
                      Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
