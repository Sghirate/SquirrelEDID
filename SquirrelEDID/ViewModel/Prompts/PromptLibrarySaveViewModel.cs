using SquirrelEDID.Model;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace SquirrelEDID.ViewModel
{
    public class PromptLibrarySaveViewModel : BaseViewModel
    {
        #region Fields
        private static string _edidFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EDIDs");
        private EDID _edid;
        private bool _exists;
        private bool _invalidFilename;
        private bool _override;
        private string _filename;
        private static string[] _fileTypes = new string[]{ ".bin", ".dat", ".txt" };
        private string _extension;
        #endregion

        #region Properties
        public EDID EDID
        {
            get { return _edid; }
            set
            {
                if (_edid == value)
                    return;

                _edid = value;
                OnPropertyChanged("EDID");
            }
        }
        public bool Exists
        {
            get { return _exists; }
            set
            {
                if (_exists == value)
                    return;

                _exists = value;
                OnPropertyChanged("Exists");
            }
        } 
        public bool InvalidFilename
        {
            get { return _invalidFilename; }
            set
            {
                if (_invalidFilename == value)
                    return;

                _invalidFilename = value;
                OnPropertyChanged("InvalidFilename");
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
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public bool Override
        {
            get { return _override; }
            set
            {
                if (_override == value)
                    return;

                _override = value;
                OnPropertyChanged("Override");
            }
        }
        public string[] FileTypes
        {
            get { return _fileTypes; }
        }
        public string Extension
        { 
            get { return _extension; }
            set
            {
                if (_extension == value)
                    return;

                _extension = value;
                OnPropertyChanged("Extension");
            }
        }
        #endregion

        #region Commands
        private ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                return _loadedCommand ?? (_loadedCommand = new RelayCommand(HandleLoadedExecuted, HandleLoadedCanExecute));
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
        private ICommand _acceptCommand;
        public ICommand AcceptCommand
        {
            get
            {
                return _acceptCommand ?? (_acceptCommand = new RelayCommand(HandleAcceptExecuted, HandleAcceptCanExecute));
            }
        }
        #endregion

        #region Methods
        private void HandleCancelExecuted(object obj)
        {
            Messenger<Prompts>.Invoke(Prompts.None);
        }

        private bool HandleCancelCanExecute(object obj)
        {
            return true;
        }

        private void HandleAcceptExecuted(object obj)
        {
            string file = Filename + Extension;
            string path = Path.Combine(_edidFolder, file);
            switch(Extension)
            {
                case ".bin":
                    EDID.SaveToHex(path);
                    break;
                case ".dat":
                    EDID.SaveToDat(path);
                    break;
                default:
                    EDID.SaveToTxt(path);
                    break;
            }
            Messenger<Prompts>.Invoke(Prompts.None);
        }

        private bool HandleAcceptCanExecute(object obj)
        {
            return PathValid();
        }

        private void HandleLoadedExecuted(object obj)
        {
            Exists = false;
            InvalidFilename = false;
            Override = false;
            Extension = FileTypes[0];
        }

        public bool HandleLoadedCanExecute(object obj)
        {
            return true;
        } 

        private bool PathValid()
        {
            Exists = false;
            InvalidFilename = false;

            if(String.IsNullOrEmpty(Filename))
                return false;

            for(int i=0;i<Filename.Length;i++)
                if(Path.GetInvalidFileNameChars().Contains(Filename[i]))
                {
                    InvalidFilename = true;
                    return false;
                }
            
            string file = Filename + Extension;
            string path = Path.Combine(_edidFolder, file);
            if(File.Exists(path))
                Exists = true;

            return !Exists || Override;
        }
        #endregion
    }
}
