using SquirrelEDID.Model;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using SquirrelEDID.Utilities.Extensions;
using System.Windows;
using System.Windows.Controls;
using SquirrelEDID.Model.Win32;

namespace SquirrelEDID.ViewModel
{
    public class EDIDViewModel : BaseViewModel
    {
        private enum EDIDAwait
        {
            None,
            ScreenSelectLoad,
            ScreenSelectOverride
        }

        #region Fields
        private EDIDAwait _await = EDIDAwait.None;
        private EDID _edid;
        private string _filename;
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

                _edid = value;
                OnPropertyChanged("EDID");
                Filename = null;
                CommandManager.InvalidateRequerySuggested();
            }
        } 
        public string Filename
        {
            get { return _filename; }
            set
            {
                if (_filename == value)
                    return;

                _filename = value;
                OnPropertyChanged("Filename");
            }
        }
        #endregion

        #region Commands
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
        private ICommand _toLibraryCommand;
        public ICommand ToLibraryCommand
        {
            get
            {
                return _toLibraryCommand ?? (_toLibraryCommand = new RelayCommand(HandleToLibraryExecuted, HandleToLibraryCanExecute));
            }
        }
        private ICommand _toFilesCommand;
        public ICommand ToFilesCommand
        {
            get
            {
                return _toFilesCommand ?? (_toFilesCommand = new RelayCommand(HandleToFilesExecuted, HandleToFilesCanExecute));
            }
        }
        private ICommand _toScreenCommand;
        public ICommand ToScreenCommand
        {
            get
            {
                return _toScreenCommand ?? (_toScreenCommand = new RelayCommand(HandleToScreenExecuted, HandleToScreenCanExecute));
            }
        }
        private ICommand _toProgrammerCommand;
        public ICommand ToProgrammerCommand
        {
            get
            {
                return _toProgrammerCommand ?? (_toProgrammerCommand = new RelayCommand(HandleToProgrammerExecuted, HandleToProgrammerCanExecute));
            }
        }
        #endregion

        #region Constructors
        public EDIDViewModel()
        {
            Messenger<DisplayInfo>.AddListener(HandleDisplayInfo);
        } 
        #endregion

        #region Methods
        private void HandleDisplayInfo(DisplayInfo di)
        {
            switch(_await)
            {
                case EDIDAwait.ScreenSelectLoad:
                    if (di.Additional == null)
                        break;

                    EDID edid = (EDID)di.Additional;
                    if (edid == null)
                        break;

                    EDID = edid;
                    break;
                case EDIDAwait.ScreenSelectOverride:
                    break;
            }
        }

        private void HandleFromLibraryExecuted(object obj)
        {
            Messenger<Prompts>.Invoke(Prompts.Library);
        }

        private bool HandleFromLibraryCanExecute(object obj)
        {
            return true;
        }

        private void HandleFromScreenExecuted(object obj)
        {
            _await = EDIDAwait.ScreenSelectLoad;
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

        private void HandleToLibraryExecuted(object obj)
        {
            IoC.Get<PromptLibrarySaveViewModel>().EDID = EDID;
            IoC.Get<PromptLibrarySaveViewModel>().Filename = Filename;
            Messenger<Prompts>.Invoke(Prompts.LibrarySave);
        }

        private bool HandleToLibraryCanExecute(object obj)
        {
            return EDID != null;
        }

        private void HandleToFilesExecuted(object obj)
        {

        }

        private bool HandleToFilesCanExecute(object obj)
        {
            return EDID != null;
        }

        private void HandleToScreenExecuted(object obj)
        {
            // INF OVERRIDE!
        }

        private bool HandleToScreenCanExecute(object obj)
        {
            return EDID != null;
        }

        private void HandleToProgrammerExecuted(object obj)
        {

        }

        private bool HandleToProgrammerCanExecute(object obj)
        {
            return (EDID != null) && IoC.Get<Programmer>().WarriorAvailable;
        }
        #endregion
    }
}
