using SquirrelEDID.Model;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Converters;
using SquirrelEDID.Utilities.Extensions;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Windows.Input;

namespace SquirrelEDID.ViewModel
{
    [SettingsObject("Data")]
    public class SettingsViewModel : BaseViewModel
    {
        #region Fields
        private string _templateEDID;
        private string _serialPrefix;
        private int _serialStart;
        private int _serialEnd;
        private string _outputDirectory;
        private bool _writeFiles;
        private bool _writeProgrammer; 
        #endregion

        #region Properties
        [Setting(Key = "TemplateEDID")]
        public string TemplateEDID
        {
            get { return _templateEDID; }
            set
            {
                if (_templateEDID == value)
                    return;

                _templateEDID = value;
                ((RelayCommand)StartCommand).RaiseCanExecuteChanged();
                OnPropertyChanged("TemplateEDID");
            }
        }
        [Setting(Key = "SerialPrefix", DefaultValue = "SQRL")]
        public string SerialPrefix
        {
            get { return _serialPrefix; }
            set
            {
                if (_serialPrefix == value)
                    return;

                _serialPrefix = value;
                OnPropertyChanged("SerialPrefix");
            }
        }
        [Setting(Key = "SerialStart", DefaultValue = 0, Converter = typeof(DoubleToIntConverter))]
        public int SerialStart
        {
            get { return _serialStart; }
            set
            {
                if (_serialStart == value)
                    return;

                _serialStart = value;
                OnPropertyChanged("SerialStart");
            }
        }
        [Setting(Key = "SerialEnd", DefaultValue = 10, Converter = typeof(DoubleToIntConverter))]
        public int SerialEnd
        {
            get { return _serialEnd; }
            set
            {
                if (_serialEnd == value)
                    return;

                _serialEnd = value;
                OnPropertyChanged("SerialEnd");
            }
        }
        [Setting(Key = "OutputDirectory")]
        public string OutputDirectory
        {
            get { return _outputDirectory; }
            set
            {
                if (_outputDirectory == value)
                    return;

                _outputDirectory = value;
                OnPropertyChanged("OutputDirectory");
            }
        }
        [Setting(Key = "WriteFiles", DefaultValue = true)]
        public bool WriteFiles
        {
            get { return _writeFiles; }
            set
            {
                if (_writeFiles == value)
                    return;

                _writeFiles = value;
                OnPropertyChanged("WriteFiles");
            }
        }
        [Setting(Key = "WriteProgrammer", DefaultValue = false)]
        public bool WriteProgrammer
        {
            get { return _writeProgrammer; }
            set
            {
                if (_writeProgrammer == value)
                    return;

                _writeProgrammer = value;
                OnPropertyChanged("WriteProgrammer");
            }
        } 
        #endregion

        #region Commands
        private ICommand _editEDIDCommand;
        public ICommand EditEDIDCommand
        {
            get
            {
                return _editEDIDCommand ?? (_editEDIDCommand = new RelayCommand(HandleEditEDIDExecuted, HandleEditEDIDCanExecute));
            }
        }
        private ICommand _loadEDIDFromLibraryCommand;
        public ICommand LoadEDIDFromLibraryCommand
        {
            get
            {
                return _loadEDIDFromLibraryCommand ?? (_loadEDIDFromLibraryCommand = new RelayCommand(HandleLoadEDIDFromLibraryExecuted, HandleLoadEDIDFromLibraryCanExecute));
            }
        }
        private ICommand _loadEDIDFromSystemCommand;
        public ICommand LoadEDIDFromSystemCommand
        {
            get
            {
                return _loadEDIDFromSystemCommand ?? (_loadEDIDFromSystemCommand = new RelayCommand(HandleLoadEDIDFromSystemExecuted, HandleLoadEDIDFromSystemCanExecute));
            }
        }
        private ICommand _loadEDIDFromProgrammerCommand;
        public ICommand LoadEDIDFromProgrammerCommand
        {
            get
            {
                return _loadEDIDFromProgrammerCommand ?? (_loadEDIDFromProgrammerCommand = new RelayCommand(HandleLoadEDIDFromProgrammerExecuted, HandleLoadEDIDFromProgrammerCanExecute));
            }
        }
        private ICommand _startCommand;
        public ICommand StartCommand
        {
            get
            {
                return _startCommand ?? (_startCommand = new RelayCommand(HandleStartExecuted, HandleStartCanExecute));
            }
        }
        private ICommand _selectOutputCommand;
        public ICommand SelectOutputCommand
        {
            get
            {
                return _selectOutputCommand ?? (_selectOutputCommand = new RelayCommand(HandleSelectOutputExecuted, HandleSelectOutputCanExecute));
            }
        }
        #endregion

        #region Constructors
        public SettingsViewModel()
        {
            Messenger<EDID>.AddListener(edid => TemplateEDID = edid.Buffer.GetHexString());
        }
        #endregion

        #region Methods
        private void HandleEditEDIDExecuted(object obj)
        {
            if (!String.IsNullOrEmpty(TemplateEDID))
            {
                byte[] data = TemplateEDID.GetBytesFromHex();
                System.Diagnostics.Trace.WriteLine(data);
                if (data != null)
                {
                    EDID edid = new EDID(data);
                    if (edid != null)
                        Messenger<EDID>.Invoke(edid);
                    System.Diagnostics.Trace.WriteLine(edid);
                }
            }
            Messenger<ApplicationStates>.Invoke(ApplicationStates.EDID);
        }

        private bool HandleEditEDIDCanExecute(object obj)
        {
            return true;
        }

        private void HandleLoadEDIDFromLibraryExecuted(object obj)
        {
        }

        private bool HandleLoadEDIDFromLibraryCanExecute(object obj)
        {
            return true;
        }

        private void HandleLoadEDIDFromSystemExecuted(object obj)
        {
        }

        private bool HandleLoadEDIDFromSystemCanExecute(object obj)
        {
            return true;
        }

        private void HandleLoadEDIDFromProgrammerExecuted(object obj)
        {
            if (IoC.Get<Programmer>().WarriorAvailable)
            {
                byte[] data = IoC.Get<Programmer>().ReadEDID();
                if (data != null && data.Length > 0)
                {
                    EDID edid = new EDID(data);
                    if (edid != null)
                        Messenger<EDID>.Invoke(edid);
                }
            }
        }

        private bool HandleLoadEDIDFromProgrammerCanExecute(object obj)
        {
            return IoC.Get<Programmer>().WarriorAvailable;
        } 

        private void HandleStartExecuted(object obj)
        {
            if (WriteProgrammer)
                Messenger<ApplicationStates>.Invoke(ApplicationStates.Programmer);
            else if (WriteFiles)
                Messenger<ApplicationStates>.Invoke(ApplicationStates.Writer);
        }

        private bool HandleStartCanExecute(object obj)
        {
            return !String.IsNullOrEmpty(_templateEDID);
        }

        private void HandleSelectOutputExecuted(object obj)
        {
            Messenger<ApplicationStates>.Invoke(ApplicationStates.FolderBrowser);
        }

        private bool HandleSelectOutputCanExecute(object obj)
        {
            return true;
        }
        #endregion
    }
}
