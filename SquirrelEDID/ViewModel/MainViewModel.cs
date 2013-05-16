using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using SquirrelEDID.View.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SquirrelEDID.ViewModel
{
    [SettingsObject("Main")]
    public class MainViewModel : BaseViewModel
    {
        #region Fields
        private double _x;
        private double _y;
        private double _w;
        private double _h;
        private static List<SolidColorBrush> _accentBrushes = new List<SolidColorBrush> 
        {
            Elysium.AccentBrushes.Blue,
            Elysium.AccentBrushes.Brown,
            Elysium.AccentBrushes.Green,
            Elysium.AccentBrushes.Lime,
            Elysium.AccentBrushes.Magenta,
            Elysium.AccentBrushes.Mango,
            Elysium.AccentBrushes.Orange,
            Elysium.AccentBrushes.Pink,
            Elysium.AccentBrushes.Purple,
            Elysium.AccentBrushes.Red,
            Elysium.AccentBrushes.Rose,
            Elysium.AccentBrushes.Sky,
            Elysium.AccentBrushes.Violet,
            Elysium.AccentBrushes.Viridian
        };
        private static List<SolidColorBrush> _contrastBrushes = new List<SolidColorBrush> 
        {
            Brushes.White,
            Brushes.Black,
            Brushes.Gray
        };
        #endregion

        #region Properties
        [Setting(Key = "X", DefaultValue = 0)]
        public double X
        {
            get { return _x; }
            set
            {
                if (_x == value)
                    return;

                _x = value;
                OnPropertyChanged("X");
            }
        }
        [Setting(Key = "Y", DefaultValue = 0)]
        public double Y
        {
            get { return _y; }
            set
            {
                if (_y == value)
                    return;

                _y = value;
                OnPropertyChanged("Y");
            }
        }
        [Setting(Key = "W", DefaultValue = 800)]
        public double W
        {
            get { return _w; }
            set
            {
                if (_w == value)
                    return;

                _w = value;
                OnPropertyChanged("W");
            }
        }
        [Setting(Key = "H", DefaultValue = 600)]
        public double H
        {
            get { return _h; }
            set
            {
                if (_h == value)
                    return;

                _h = value;
                OnPropertyChanged("H");
            }
        }
        public List<SolidColorBrush> AccentBrushes { get { return _accentBrushes; } }
        public List<SolidColorBrush> ContrastBrushes { get { return _contrastBrushes; } }
        #endregion

        #region Commands
        private ICommand _themeLightCommand;
        public ICommand ThemeLightCommand
        {
            get
            {
                return _themeLightCommand ?? (_themeLightCommand = new RelayCommand(HandleThemeLightExecuted));
            }
        }
        private ICommand _themeDarkCommand;
        public ICommand ThemeDarkCommand
        {
            get
            {
                return _themeDarkCommand ?? (_themeDarkCommand = new RelayCommand(HandleThemeDarkExecuted));
            }
        }
        private ICommand _accentColorCommand;
        public ICommand AccentColorCommand
        {
            get
            {
                return _accentColorCommand ?? (_accentColorCommand = new RelayCommand(HandleAccentColorExecuted));
            }
        }
        private ICommand _contrastColorCommand;
        public ICommand ContrastColorCommand
        {
            get
            {
                return _contrastColorCommand ?? (_contrastColorCommand = new RelayCommand(HandleContrastColodExecuted));
            }
        }
        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                return _aboutCommand ?? (_aboutCommand = new RelayCommand(HandleAboutExecuted));
            }
        }
        #endregion

        #region Methods
        private void HandleAboutExecuted(object obj)
        {
            Messenger<ApplicationStates>.Invoke(ApplicationStates.About);
        }

        private void HandleThemeLightExecuted(object obj)
        {
            App app = (App)App.Current;
            app.Theme = Elysium.Theme.Light;
            app.ApplyTheme();
        }

        private void HandleThemeDarkExecuted(object obj)
        {
            App app = (App)App.Current;
            app.Theme = Elysium.Theme.Dark;
            app.ApplyTheme();
        } 
        private void HandleAccentColorExecuted(object obj)
        {
            if (!(obj is SolidColorBrush))
                return;

            SolidColorBrush brush = (SolidColorBrush)obj;
            App app = (App)App.Current;
            app.AccentBrush = brush;
            app.ApplyTheme();
        }
        private void HandleContrastColodExecuted(object obj)
        {
            if (!(obj is SolidColorBrush))
                return;

            SolidColorBrush brush = (SolidColorBrush)obj;
            App app = (App)App.Current;
            app.ContrastBrush = brush;
            app.ApplyTheme();
        }
        #endregion
    }
}
