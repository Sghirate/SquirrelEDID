using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using SquirrelEDID.View.Controls;
using SquirrelEDID.ViewModel;
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

namespace SquirrelEDID
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SizeChanged += (s, e) => Snap();
            LocationChanged += (s, e) => Snap();

            Messenger<ApplicationStates>.AddListener(GotoState);

            Loaded += (s, e) => GotoState(ApplicationStates.Welcome);
        }

        private void GotoState(ApplicationStates state)
        {
            ApplicationStates cur = App.CurrentState;

            FrameworkElement view = (FrameworkElement)IoC.Repository[state.ToString() + "View"];
            if (view == null)
                return;

            //BaseViewModel vm = (BaseViewModel)IoC.Repository[state.ToString() + "ViewModel"];
            //if (vm != null)
            //    view.DataContext = vm;

            SlideDirection direction = SlideDirection.Left;
            var dir = App.SlideDirections.Where(s => s.Item1 == cur && s.Item2 == state).FirstOrDefault();
            if (dir != null)
                direction = dir.Item3;

            slide.Slide(view, direction);

            App.CurrentState = state;
        }

        private void Snap()
        {
            if (Width > SystemParameters.VirtualScreenWidth)
                Width = SystemParameters.VirtualScreenWidth;
            if (Height > SystemParameters.VirtualScreenHeight)
                Height = SystemParameters.VirtualScreenHeight;

            if (Left < SystemParameters.VirtualScreenLeft)
                Left = SystemParameters.VirtualScreenLeft;
            if (Top < SystemParameters.VirtualScreenTop)
                Top = SystemParameters.VirtualScreenTop;

            if (Left + Width > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)
                Left = (SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth) - Width;
            if (Top + Height > SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight)
                Top = (SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight) - Height;
        }
    }
}
