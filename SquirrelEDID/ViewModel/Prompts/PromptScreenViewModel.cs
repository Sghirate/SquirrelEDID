using SquirrelEDID.Model;
using SquirrelEDID.Model.Win32;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace SquirrelEDID.ViewModel
{
    public class PromptScreenViewModel : BaseViewModel
    {
        #region Fields
        private List<DisplayInfo> _displays;
        private bool _loading;
        private DisplayInfo _selectedDisplay;
        #endregion

        #region Properties
        public List<DisplayInfo> Displays
        {
            get { return _displays; }
            set
            {
                if (_displays == value)
                    return;

                _displays = value;
                OnPropertyChanged("Displays");
            }
        }
        public bool Loading
        {
            get { return _loading; }
            set
            {
                if (_loading == value)
                    return;

                _loading = value;
                OnPropertyChanged("Loading");
            }
        }
        public DisplayInfo SelectedDisplay
        {
            get { return _selectedDisplay; }
            set
            {
                if (_selectedDisplay == value)
                    return;

                _selectedDisplay = value;
                OnPropertyChanged("SelectedDisplay");
            }
        }
        #endregion

        #region Commands
        private ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                return _loadedCommand ?? (_loadedCommand = new RelayCommand(HandleLoadedExecuted));
            }
        }
        private ICommand _acceptCommand;
        public ICommand AcceptCommand
        {
            get
            {
                return _acceptCommand ?? (_acceptCommand = new RelayCommand(HandleAcceptExecuted, HandleAcceptCanExecute));
            }
        }
        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(HandleCancelExecuted, HandleCancelCanExecute));
            }
        } 
        #endregion

        #region Methods
        private void HandleLoadedExecuted(object obj)
        {
            Task.Factory.StartNew(() =>
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() => { Loading = true; }), null);
                List<DisplayInfo> displays = new List<DisplayInfo>();
                foreach (var d in Display.GetDisplays())
                {
                    if (d.EDID != null && d.EDID.Length > 0)
                    {
                        d.Additional = new EDID(d.EDID);
                        displays.Add(d);
                    }
                }
                App.Current.Dispatcher.BeginInvoke(new Action(() => { Displays = displays; }), null);
                App.Current.Dispatcher.BeginInvoke(new Action(() => { Loading = false; }), null);
            });
        }

        private void HandleAcceptExecuted(object obj)
        {
            Messenger<DisplayInfo>.Invoke(SelectedDisplay);
            Messenger<Prompts>.Invoke(Prompts.None);
        }

        private bool HandleAcceptCanExecute(object obj)
        {
            return SelectedDisplay != null;
        }

        private void HandleCancelExecuted(object obj)
        {
            Messenger<Prompts>.Invoke(Prompts.None);
        }

        private bool HandleCancelCanExecute(object obj)
        {
            return true;
        } 
        #endregion
    }
}
