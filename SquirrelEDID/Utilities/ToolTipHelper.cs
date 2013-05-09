using System;
using System.Windows;

namespace SquirrelEDID.Utilities
{
    public class ToolTipHelper
    {
        #region DependencyProperties
        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached("ToolTip", typeof(string), typeof(ToolTipHelper), new PropertyMetadata(OnToolTipChanged)); 
        #endregion

        #region Methods
        public static string GetToolTip(DependencyObject obj)
        {
            return (string)obj.GetValue(ToolTipProperty);
        }

        public static void SetToolTip(DependencyObject obj, string value)
        {
            obj.SetValue(ToolTipProperty, value);
        }

        public static void OnToolTipChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is FrameworkElement))
                return;

            FrameworkElement ele = (FrameworkElement)obj;
            ele.MouseMove -= ele_MouseMove;
            ele.MouseLeave -= ele_MouseLeave;

            if (!String.IsNullOrEmpty((string)e.NewValue))
            {
                ele.MouseMove += ele_MouseMove;
                ele.MouseLeave += ele_MouseLeave;
            }
        }

        static void ele_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DependencyObject obj = (DependencyObject)sender;
            if (sender == null)
                return;

            Application.Current.Resources["tooltip"] = GetToolTip(obj);
        }

        static void ele_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Application.Current.Resources["tooltip"] = "";
        } 
        #endregion
    }
}
