using SquirrelEDID.Model;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SquirrelEDID.ViewModel
{
    public class EDIDViewModel : BaseViewModel
    {
        #region Fields
        private EDID _edid; 
        #endregion

        #region Properties
        public EDID EDID
        {
            get { return _edid; }
            set
            {
                if (_edid == value)
                    return;

                System.Diagnostics.Trace.WriteLine(value.Length);
                _edid = value;
                OnPropertyChanged("EDID");
            }
        } 
        #endregion

        #region Commands
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

        #region Constructors
        public EDIDViewModel()
        {
            System.Diagnostics.Trace.WriteLine("INIT");
            Messenger<EDID>.AddListener(edid => { if (edid != null) EDID = edid; System.Diagnostics.Trace.WriteLine("CALL"); });
        } 
        #endregion

        #region Methods
        private void HandleAcceptExecuted(object obj)
        {
            Messenger<EDID>.Invoke(_edid);
            Messenger<ApplicationStates>.Invoke(ApplicationStates.Settings);
        }

        private bool HandleAcceptCanExecute(object obj)
        {
            return true;
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
