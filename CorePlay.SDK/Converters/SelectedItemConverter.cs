using Avalonia.Data.Converters;
using Avalonia.Media;
using CorePlay.SDK.Models;
using System.Globalization;

namespace CorePlay.SDK.Converters
{
    public class SelectedItemConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count == 2 && values[0] is ImageGalleryItem selectedItem && values[1] is ImageGalleryItem currentItem)
            {
                // Implement your logic to compare the selected item with the current item
                // Example: If the current item is selected, return a color, else return null or default value
                return Equals(selectedItem, currentItem) ? Brushes.White : Brushes.Transparent;
            }

            return Brushes.Transparent; // Default brush if the conversion fails
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}