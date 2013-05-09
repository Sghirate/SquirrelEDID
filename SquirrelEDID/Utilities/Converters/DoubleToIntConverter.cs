using System;
using System.Windows.Data;

namespace SquirrelEDID.Utilities.Converters
{
    public class DoubleToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is double))
                return value;

            double d = (double)value;
            if (d < int.MinValue)
                return int.MinValue;
            else if (d > int.MaxValue)
                return int.MaxValue;
            else
                return (int)d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is int))
                return value;

            int i = (int)value;

            return (double)i;
        }
    }
}
