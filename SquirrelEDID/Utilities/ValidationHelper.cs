using SquirrelEDID.Resources;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SquirrelEDID.Utilities
{
    public class ValidationHelper : ValidationRule
    {
        #region Fields
        private static char[] _validHex = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', ' ', ',', 'x' };
        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.RegisterAttached("MinValue", typeof(double), typeof(ValidationHelper), new UIPropertyMetadata(double.NaN));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.RegisterAttached("MaxValue", typeof(double), typeof(ValidationHelper), new UIPropertyMetadata(double.NaN));
        public static readonly DependencyProperty MinLengthProperty = DependencyProperty.RegisterAttached("MinLength", typeof(int), typeof(ValidationHelper), new UIPropertyMetadata(-1));
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength", typeof(int), typeof(ValidationHelper), new UIPropertyMetadata(-1));
        public static readonly DependencyProperty OnlyCharactersProperty = DependencyProperty.RegisterAttached("OnlyCharactersProperty", typeof(bool), typeof(ValidationHelper), new UIPropertyMetadata(false));
        public static readonly DependencyProperty OnlyHexProperty = DependencyProperty.RegisterAttached("OnlyHex", typeof(bool), typeof(ValidationHelper), new UIPropertyMetadata(false));
        public static readonly DependencyProperty NoEmptyValueProperty = DependencyProperty.RegisterAttached("NoEmptyValue", typeof(bool), typeof(ValidationHelper), new UIPropertyMetadata(false));
        public static readonly DependencyProperty ValidationTypeProperty = DependencyProperty.RegisterAttached("ValidationType", typeof(Type), typeof(ValidationRule), new UIPropertyMetadata(null, OnValidationTypeChanged)); 
        #endregion

        #region Properties
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public bool OnlyCharacters { get; set; }
        public bool OnlyHex { get; set; }
        public bool NoEmptyValue { get; set; }
        public Type ValidationType { get; set; }
        #endregion

        #region Constructors
        public ValidationHelper() 
        { 
        }
        
        public ValidationHelper(Type validationType)
        {
            ValidationType = validationType;
        }

        public ValidationHelper(Type validationType, double minValue, double maxValue)
        {
            ValidationType = validationType;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public ValidationHelper(Type validationType, int minLength, int maxLength, bool onlyCharacters, bool onlyHex, bool noEmptyValue)
        {
            ValidationType = validationType;
            MinLength = minLength;
            MaxLength = maxLength;
            OnlyCharacters = onlyCharacters;
            OnlyHex = onlyHex;
            NoEmptyValue = noEmptyValue;
        }

        public ValidationHelper(Type validationType, double minValue, double maxValue, int minLength, int maxLength, bool onlyCharacters, bool onlyHex, bool noEmptyValue)
        {
            ValidationType = validationType;
            MinValue = minValue;
            MaxValue = maxValue;
            MinLength = minLength;
            MaxLength = maxLength;
            OnlyCharacters = onlyCharacters;
            OnlyHex = onlyHex;
            NoEmptyValue = noEmptyValue;
        }
        #endregion

        #region Methods
        public static double GetMinValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MinValueProperty);
        }

        public static void SetMinValue(DependencyObject obj, double value)
        {
            obj.SetValue(MinValueProperty, value);
        }

        public static double GetMaxValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MaxValueProperty);
        }

        public static void SetMaxValue(DependencyObject obj, double value)
        {
            obj.SetValue(MaxValueProperty, value);
        }

        public static int GetMinLength(DependencyObject obj)
        {
            return (int)obj.GetValue(MinLengthProperty);
        }

        public static void SetMinLength(DependencyObject obj, int value)
        {
            obj.SetValue(MinLengthProperty, value);
        }

        public static int GetMaxLength(DependencyObject obj)
        {
            return (int)obj.GetValue(MaxLengthProperty);
        }

        public static void SetMaxLength(DependencyObject obj, int value)
        {
            obj.SetValue(MaxLengthProperty, value);
        }

        public static bool GetOnlyCharacters(DependencyObject obj)
        {
            return (bool)obj.GetValue(OnlyCharactersProperty);
        }

        public static void SetOnlyCharacters(DependencyObject obj, bool value)
        {
            obj.SetValue(OnlyCharactersProperty, value);
        }

        public static bool GetOnlyHex(DependencyObject obj)
        {
            return (bool)obj.GetValue(OnlyHexProperty);
        }

        public static void SetOnlyHex(DependencyObject obj, bool value)
        {
            obj.SetValue(OnlyHexProperty, value);
        }

        public static Type GetValidationType(DependencyObject obj)
        {
            return (Type)obj.GetValue(ValidationTypeProperty);
        }

        public static void SetValidationType(DependencyObject obj, Type value)
        {
            obj.SetValue(ValidationTypeProperty, value);
        } 

        public static bool GetNoEmptyValue(DependencyObject obj)
        {
            return (bool)obj.GetValue(NoEmptyValueProperty);
        }

        public static void SetNoEmptyValue(DependencyObject obj, bool value)
        {
            obj.SetValue(NoEmptyValueProperty, value);
        }

        private static void OnValidationTypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var element = obj as FrameworkElement;
            if (element == null) return;

            // When the element has loaded.
            element.Loaded += (s, e) =>
            {
                var valid = new ValidationHelper(GetValidationType(obj), GetMinValue(obj), GetMaxValue(obj), GetMinLength(obj), GetMaxLength(obj), GetOnlyCharacters(obj), GetOnlyHex(obj), GetNoEmptyValue(obj));
                Binding bind = BindingOperations.GetBinding(obj, TextBox.TextProperty);
                if (bind != null)
                    if (!bind.ValidationRules.Contains(valid))
                        bind.ValidationRules.Add(valid);
            };
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            bool valid = true;
            string error = String.Empty;
            try
            {
                if (ValidationType != null)
                    Convert.ChangeType(value, ValidationType);

                string str = value.ToString();
                double d;
                if (double.TryParse(str, out d))
                {
                    if (MaxValue != double.NaN && d > MaxValue)
                    {
                        valid = false;
                        error += GetErr("INV_MaxValue", MaxValue);
                    }
                    if (MinValue != double.NaN && d < MinValue)
                    {
                        valid = false;
                        error += GetErr("INV_MinValue", MinValue);
                    }
                }

                if (MinLength > 0 && str.Length < MinLength)
                {
                    valid = false;
                    error += GetErr("INV_MinLength", MinLength);
                }
                if (MaxLength > 0 && str.Length > MaxLength)
                {
                    valid = false;
                    error += GetErr("INV_MaxLength", MaxLength);
                }
                if (OnlyHex && !String.IsNullOrEmpty(str))
                {
                    string lower = str.ToLower();
                    for (int i = 0; i < lower.Length; i++)
                    {
                        bool v = false;
                        for (int j = 0; j < _validHex.Length; j++)
                            if (_validHex[j] == lower[i])
                                v = true;

                        if (!v)
                        {
                            valid = false;
                            error += GetErr("INV_Hex");
                            break;
                        }
                    }
                        
                }
                if (OnlyCharacters && !String.IsNullOrEmpty(str))
                {
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (!Char.IsLetter(str[i]))
                        {
                            valid = false;
                            error += GetErr("INV_Characters");
                            break;
                        }
                    }
                }
                if(NoEmptyValue && String.IsNullOrEmpty(str.Trim()))
                {
                    valid = false;
                    error += GetErr("INV_Empty");
                }
            }
            catch (Exception ex)
            {
                valid = false;
                error += GetErr("INV_Type", ValidationType);
            }

            return new ValidationResult(valid, error.Trim());
        }

        private string GetErr(string key, object args = null)
        {
            string msg = Strings.ResourceManager.GetString(key);
            if (String.IsNullOrEmpty(msg))
                msg = "#" + key;
            if (msg.Contains("{0}"))
                msg = String.Format(msg, args);
            msg += "\r\n";
            return msg;
        }
        #endregion
    }
}
