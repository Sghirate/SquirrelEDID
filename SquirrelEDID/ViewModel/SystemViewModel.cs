using SquirrelEDID.Model;
using SquirrelEDID.Model.Win32;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SquirrelEDID.ViewModel
{
    public class SystemViewModel : BaseViewModel
    {
        #region Fields
        private List<DisplayInfo> _displays = new List<DisplayInfo>();
        private DisplayInfo _selectedDisplay;
        private bool _loading = false;
        #endregion

        #region Properties
        public DisplayInfo SelectedDisplay
        {
            get { return _selectedDisplay; }
            set
            {
                if (_selectedDisplay == value)
                    return;

                _selectedDisplay = value;
                CommandManager.InvalidateRequerySuggested();
                OnPropertyChanged("SelectedDisplay");
            }
        }
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
        #endregion

        #region Commands
        private ICommand _reloadDisplaysCommand;
        public ICommand ReloadDisplaysCommand
        {
            get
            {
                return _reloadDisplaysCommand ?? (_reloadDisplaysCommand = new RelayCommand(HandleReloadDisplaysExecuted, HandleReloadDisplaysCanExecute));
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
        private ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                return _loadedCommand ?? (_loadedCommand = new RelayCommand(HandleLoadedExecuted, HandleLoadedCanExecute));
            }
        }
        #endregion

        #region Methods
        private void HandleReloadDisplaysExecuted(object obj)
        {
            LoadDisplays();
        }

        private bool HandleReloadDisplaysCanExecute(object obj)
        {
            return true;
        }

        private void HandleAcceptExecuted(object obj)
        {
            if (SelectedDisplay != null && SelectedDisplay.Additional != null && (SelectedDisplay.Additional is EDID))
                Messenger<EDID>.Invoke((EDID)SelectedDisplay.Additional);
            Messenger<ApplicationStates>.Invoke(ApplicationStates.Settings);
        }

        private bool HandleAcceptCanExecute(object obj)
        {
            return SelectedDisplay != null;
        }

        private void HandleCancelExecuted(object obj)
        {
            Messenger<ApplicationStates>.Invoke(ApplicationStates.Settings);
        }

        private bool HandleCancelCanExecute(object obj)
        {
            return true;
        }

        private void HandleLoadedExecuted(object obj)
        {
            LoadDisplays();
        }

        private bool HandleLoadedCanExecute(object obj)
        {
            return true;
        }

        private void LoadDisplays()
        {
            SelectedDisplay = null;
            Task.Factory.StartNew(() =>
            {
                if (_loading)
                    return;
                _loading = true;
                List<DisplayInfo> displays = new List<DisplayInfo>();
                foreach (var d in Display.GetDisplays())
                {
                    d.Additional = new EDID(d.EDID);
                    displays.Add(d);
                }
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { Displays = displays; }), null);
                _loading = false; 
            });
        } 
        #endregion
    }
}
