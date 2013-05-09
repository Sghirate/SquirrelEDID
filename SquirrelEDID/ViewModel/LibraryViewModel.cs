using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SquirrelEDID.ViewModel
{
    public class LibraryViewModel : BaseViewModel
    {
        #region Fields
        private object _selectedEDID; 
        #endregion

        #region Properties
        public object SelectedEDID
        {
            get { return _selectedEDID; }
            set
            {
                if (_selectedEDID == value)
                    return;

                _selectedEDID = value;
                ((RelayCommand)AcceptCommand).RaiseCanExecuteChanged();
                OnPropertyChanged("SelectedEDID");
            }
        } 
        #endregion

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

        #region Methods
        private void HandleAcceptExecuted(object obj)
        {
            Messenger<ApplicationStates>.Invoke(ApplicationStates.Settings);
        }

        private bool HandleAcceptCanExecute(object obj)
        {
            return SelectedEDID != null;
        }

        private void HandleCancelExecuted(object obj)
        {
            Messenger<ApplicationStates>.Invoke(ApplicationStates.Settings);
        }

        private bool HandleCancelCanExecute(object obj)
        {
            return true;
        } 
        #endregion
    }
}
