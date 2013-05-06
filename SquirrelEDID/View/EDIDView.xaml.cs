using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using SquirrelEDID.View.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SquirrelEDID.View
{
    /// <summary>
    /// Interaction logic for EDIDView.xaml
    /// </summary>
    public partial class EDIDView : UserControl
    {
        public EDIDView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Messenger<FrameworkElement, SlideDirection>.Invoke(IoC.Get<WelcomeView>(), SlideDirection.Right);
        }
    }
}
