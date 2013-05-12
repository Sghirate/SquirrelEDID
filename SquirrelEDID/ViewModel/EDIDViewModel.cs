using SquirrelEDID.Model;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using SquirrelEDID.Utilities.Extensions;

namespace SquirrelEDID.ViewModel
{
    public class EDIDViewModel : BaseViewModel
    {
        #region Fields
        private EDID _edid; 
        #endregion

        #region Properties
        [Setting("EDID")]
        public string TemplateEDID
        {
            get { return _edid.Buffer.GetHexString(); }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                byte[] buffer = value.GetBytesFromHex(128);
                if (buffer == null || buffer.Length != 128)
                    return;

                EDID edid = new EDID(buffer);
                if (edid == null)
                    return;

                EDID = edid;
            }
        }
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
        private ICommand _fromLibraryCommand;
        public ICommand FromLibraryCommand
        {
            get
            {
                return _fromLibraryCommand ?? (_fromLibraryCommand = new RelayCommand(HandleFromLibraryExecuted, HandleFromLibraryCanExecute));
            }
        }
        private ICommand _fromScreenCommand;
        public ICommand FromScreenCommand
        {
            get
            {
                return _fromScreenCommand ?? (_fromScreenCommand = new RelayCommand(HandleFromScreenExecuted, HandleFromScreenCanExecute));
            }
        }
        private ICommand _fromProgrammerCommand;
        public ICommand FromProgrammerCommand
        {
            get
            {
                return _fromProgrammerCommand ?? (_fromProgrammerCommand = new RelayCommand(HandleFromProgrammerExecuted, HandleFromProgrammerCanExecute));
            }
        }
        #endregion

        #region Constructors
        public EDIDViewModel()
        {
            Messenger<EDID>.AddListener(edid => { if (edid != null) EDID = edid; System.Diagnostics.Trace.WriteLine("CALL"); });
        } 
        #endregion

        #region Methods
        private void HandleFromLibraryExecuted(object obj)
        {

        }

        private bool HandleFromLibraryCanExecute(object obj)
        {
            return true;
        }

        private void HandleFromScreenExecuted(object obj)
        {
            Messenger<Prompts>.Invoke(Prompts.Screen);
        }

        private bool HandleFromScreenCanExecute(object obj)
        {
            return true;
        }

        private void HandleFromProgrammerExecuted(object obj)
        {
            byte[] buffer = IoC.Get<Programmer>().ReadEDID();
            if (buffer == null || buffer.Length < 1)
            {
                IoC.Get<PromptProgrammerViewModel>().State = ProgrammerStates.ReadFailed;
                Messenger<Prompts>.Invoke(Prompts.Programmer);
                return;
            }

            EDID edid = new EDID(buffer);
            if (edid == null)
            {
                IoC.Get<PromptProgrammerViewModel>().State = ProgrammerStates.ReadFailed;
                Messenger<Prompts>.Invoke(Prompts.Programmer);
                return;
            }

            EDID = edid;
            IoC.Get<PromptProgrammerViewModel>().State = ProgrammerStates.ReadSuccess;
            Messenger<Prompts>.Invoke(Prompts.Programmer);
        }

        private bool HandleFromProgrammerCanExecute(object obj)
        {
            return IoC.Get<Programmer>().WarriorAvailable;
        }

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
