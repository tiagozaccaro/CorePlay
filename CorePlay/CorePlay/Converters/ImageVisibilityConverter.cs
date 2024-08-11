using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CorePlay.Converters
{
    public class ImageVisibilityConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            string? imageSource = values[0] as string;
            return imageSource != null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
