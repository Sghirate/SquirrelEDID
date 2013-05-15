using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace SquirrelEDID.Utilities.Converters
{/// <summary>
    /// Used to convert a boolean to any other Type.
    /// </summary>
    /// <typeparam name="T">Target Type</typeparam>
    public class BoolToValueConverter<T> : MarkupExtension, IValueConverter
    {
        #region Properties
        /// <summary>
        /// During Conversion: If the given boolean equals false, this property's value will be returned.
        /// </summary>
        public T FalseValue { get; set; }
        /// <summary>
        /// During Conversion: If the given boolean equals true, this property's value will be returned.
        /// </summary>
        public T TrueValue { get; set; }
        #endregion

        #region Constructors
        public BoolToValueConverter() // Only here, because I dislike the XAML-Warning
        {
    
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert boolean value to any Type
        /// </summary>
        /// <param name="value">Input boolean for conversion</param>
        /// <param name="targetType">unused</param>
        /// <param name="parameter">unused</param>
        /// <param name="culture">ununsed</param>
        /// <returns>If the input value equals true, TrueValue will be returned, otherwise FalseValue</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                return ConvertBack(value, targetType, parameter, culture);

            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Convert any Type to a boolean, indication whether value equals TrueValue.
        /// </summary>
        /// <param name="value">Any object (with any type)</param>
        /// <param name="targetType">unused</param>
        /// <param name="parameter">unused</param>
        /// <param name="culture">unused</param>
        /// <returns>True, if the given input value equals TrueType's value, otherwise false</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                return Convert(value, targetType, parameter, culture);

            return value != null ? value.Equals(TrueValue) : false;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        #endregion
    }

    public class BoolToVisibilityConverter : BoolToValueConverter<Visibility> { public BoolToVisibilityConverter() : base() { } }
    public class BoolToIntConverter : BoolToValueConverter<int> { public BoolToIntConverter() : base() { } }
    public class BoolToStringConverter : BoolToValueConverter<string> { public BoolToStringConverter() : base() { } }
    public class BoolToBrushConverter : BoolToValueConverter<Brush> { public BoolToBrushConverter() : base() { } }
    public class BoolToDoubleConverter : BoolToValueConverter<double> { public BoolToDoubleConverter() : base() { } }
    public class BoolToBoolConverter : BoolToValueConverter<bool> { public BoolToBoolConverter() : base() { } }
    public class BoolToImageSourceConverter : BoolToValueConverter<ImageSource> { public BoolToImageSourceConverter() : base() { } }
}
