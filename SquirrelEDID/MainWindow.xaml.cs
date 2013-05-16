using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using SquirrelEDID.View.Controls;
using SquirrelEDID.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class MainWindow : Elysium.Controls.Window
    {
        private static string _win = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        private static string _segoeUI = _win + "\\Fonts\\SegoeUI.ttf";
        private static string _verdanta = _win + "\\Fonts\\Verdana.ttf";
        Prompts _cPrompt = Prompts.None;
        Queue<Prompts> _prompts = new Queue<Prompts>();

        public MainWindow()
        {
            InitializeComponent();

            SizeChanged += (s, e) => Snap();
            LocationChanged += (s, e) => Snap();

            Messenger<ApplicationStates>.AddListener(GotoState);
            Messenger<Prompts>.AddListener(ShowPrompt);

            App.CurrentState = ApplicationStates.Back;
            Loaded += (s, e) => GotoState(ApplicationStates.Welcome);
        }

        private Prompts GetTargetPrompt(Prompts p)
        {
            if (_cPrompt == Prompts.None)
                return p;

            if (p != Prompts.None)
                _prompts.Enqueue(p);

            if (_prompts.Count > 0)
                p = _prompts.Dequeue();

            return p;
        }

        private void ShowPrompt(Prompts p)
        {
            p = GetTargetPrompt(p);

            if (p == Prompts.None)
            {
                prompt.Slide(null, SlideDirection.Down);
                _cPrompt = p;
                return;
            }

            FrameworkElement view = (FrameworkElement)IoC.Repository["Prompt" + p.ToString() + "View"];
            if (view == null)
                return;

            prompt.Slide(view, SlideDirection.Up, true);
            _cPrompt = p;
        }

        private void GotoState(ApplicationStates state)
        {
            ApplicationStates cur = App.CurrentState;

            if (state == ApplicationStates.Back)
            {
                if (App.PreviousState == ApplicationStates.Back)
                    return;

                state = App.PreviousState;
            }

            if (state == App.CurrentState)
                return;

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

            App.PreviousState = App.CurrentState;
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

        private void Glyph_Initialized(object sender, EventArgs e)
        {
            if (!(sender is Glyphs))
                return;

            Glyphs g = (Glyphs)sender;
            g.FontUri = new Uri(File.Exists(_segoeUI) ? _segoeUI : _verdanta);
        }
    }
}
