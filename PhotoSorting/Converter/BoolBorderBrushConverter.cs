using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PhotoSorting.Converter
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BoolBorderBrushConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool input))
                return new SolidColorBrush(Colors.Transparent);

            return new SolidColorBrush(input ? Colors.YellowGreen : Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}