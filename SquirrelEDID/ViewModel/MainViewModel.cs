using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using SquirrelEDID.View.Controls;
using System.Windows;

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
        #endregion
    }
}
