using SquirrelEDID.Model;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace SquirrelEDID.Utilities.Converters
{
    public class EstablishedTimingConverter : MarkupExtension, IValueConverter
    { 
        #region Fields
        private static int _store;
        #endregion

        #region Properties
        public int Bit { get; set; }
        public bool Name { get; set; } 
        #endregion

        #region Constructors
        public EstablishedTimingConverter()
        { }
        #endregion

        #region Methods
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Bit < 0 || Bit >= 24)
                return null;

            if (Name)
                return EDID.EstablishedTimings[Bit];

            if (!(value is int))
                return null;

            _store = (int)value;
            int mask = (1 << Bit);
            return (_store & mask) == mask;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Name)
                return _store;

            if (Bit < 0 || Bit >= 24)
                return _store;

            if (!(value is bool))
                return _store;

            bool b = (bool)value;
            int mask = (1 << Bit);

            if (b)
                _store = (_store | mask);
            else
                _store = (_store & ~mask);

            return _store;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        } 
        #endregion
    }
}
