using SquirrelEDID.Utilities.Extensions;
using System;
using System.Windows.Data;

namespace SquirrelEDID.Utilities.Converters
{
    public class ByteToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is byte))
                return null;

            byte b = (byte)value;
            return b.ToString("X2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is string))
                return 0;

            string str = (string)value;
            if (String.IsNullOrEmpty(str))
                return 0;

            byte[] buff = str.GetBytesFromHex();
            if (buff == null || buff.Length < 1)
                return 0;

            return buff[0];
        }
    }
}
