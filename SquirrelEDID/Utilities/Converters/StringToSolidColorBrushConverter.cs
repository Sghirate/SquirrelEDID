using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SquirrelEDID.Utilities.Converters
{
    public class StringToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is string))
                return null;

            string str = (string)value;
            Color clr = (Color)ColorConverter.ConvertFromString(str);
            return new SolidColorBrush(clr);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is SolidColorBrush))
                return null;

            SolidColorBrush brush = (SolidColorBrush)value;
            Color c = brush.Color;
            return String.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
        }
    }
}
