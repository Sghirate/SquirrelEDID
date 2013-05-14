using Elysium;
using System;
using System.Windows.Data;

namespace SquirrelEDID.Utilities.Converters
{
    public class StringToThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is string))
                return null;

            string str = (string)value;
            if (str != null && str.ToLower().Equals("light"))
                return Theme.Light;
            else
                return Theme.Dark;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Theme))
                return null;

            Theme theme = (Theme)value;
            return theme.ToString();
        }
    }
}
