using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using SquirrelEDID.View.Controls;
using System.Windows;
using System.Windows.Controls;

namespace SquirrelEDID.View
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : UserControl
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Messenger<FrameworkElement, SlideDirection>.Invoke(IoC.Get<SettingsView>(), SlideDirection.Left);
        }
    }
}
