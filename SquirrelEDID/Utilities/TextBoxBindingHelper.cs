using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SquirrelEDID.Utilities
{/// <summary>
    /// Also:
    /// Supports a PropertyChanged-Trigger for DataBindings
    /// in Silverlight. Works just for TextBoxes
    /// (C) Thomas Claudius Huber 2009
    /// http://www.thomasclaudiushuber.com
    /// </summary>
    public class TextBoxBindingHelper
    {
        #region Fields
        private static Dictionary<TextBox, DispatcherTimer> _timers = new Dictionary<TextBox, DispatcherTimer>();
        #endregion

        #region DependencyProperties
        // Using a DependencyProperty as the backing store for …
        public static readonly DependencyProperty UpdateSourceOnChangeProperty = DependencyProperty.RegisterAttached("UpdateSourceOnChange",
            typeof(bool), typeof(TextBoxBindingHelper), new PropertyMetadata(false, OnPropertyChanged));
        public static readonly DependencyProperty UpdateDelayPropery = DependencyProperty.RegisterAttached("UpdateDelay",
            typeof(double), typeof(TextBoxBindingHelper), new PropertyMetadata(0.0));
        #endregion

        #region Methods
        public static bool GetUpdateSourceOnChange(DependencyObject obj)
        {
            return (bool)obj.GetValue(UpdateSourceOnChangeProperty);
        }

        public static void SetUpdateSourceOnChange(DependencyObject obj, bool value)
        {
            obj.SetValue(UpdateSourceOnChangeProperty, value);
        }

        public static double GetUpdateDelay(DependencyObject obj)
        {
            return (double)obj.GetValue(UpdateDelayPropery);
        }

        public static void SetUpdateDelay(DependencyObject obj, double value)
        {
            obj.SetValue(UpdateDelayPropery, value);
        }

        private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var txt = obj as TextBox;
            if (txt == null)
                return;
            if ((bool)e.NewValue)
            {
                txt.TextChanged += OnTextChanged;
            }
            else
            {
                txt.TextChanged -= OnTextChanged;
            }
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt == null)
                return;

            if (_timers == null)
                return;

            if (_timers.ContainsKey(txt))
            {
                _timers[txt].Stop();
            }
            else
            {
                double d = (double)txt.GetValue(UpdateDelayPropery);

                DispatcherTimer t = new DispatcherTimer();
                t.Interval = TimeSpan.FromSeconds(d);
                t.Tick += (object tS, EventArgs tE) =>
                {
                    var be = txt.GetBindingExpression(TextBox.TextProperty);
                    if (be != null)
                    {
                        be.UpdateSource();
                    }
                    t.Stop();
                };
                _timers.Add(txt, t);
            }
            _timers[txt].Start();
        }
        #endregion
    }
}
