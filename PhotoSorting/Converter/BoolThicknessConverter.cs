using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PhotoSorting.Converter
{
    [ValueConversion(typeof(bool), typeof(Thickness))]
    public class BoolThicknessConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool input))
                return new Thickness(0);

            if (parameter is string thickness && double.TryParse(thickness, out var thicknessValue))
                return new Thickness(input ? thicknessValue : 0);

            return new Thickness(0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}