using System;
using System.Globalization;
using System.Windows.Data;

namespace PhotoSorting.Converters
{
    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double doubleValue))
                return 0;

            if (!(parameter is string stringParameter))
                return 0;

            if (!double.TryParse(stringParameter, out var doubleParameter))
                return 0;

            return doubleValue * doubleParameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("Cannot convert back...");
        }
    }
}