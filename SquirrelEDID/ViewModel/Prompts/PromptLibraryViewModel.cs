using Microsoft.Win32;
using SquirrelEDID.Model;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Extensions;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SquirrelEDID.ViewModel
{
    public class PromptLibraryViewModel : BaseViewModel
    {
        #region Fields
        private static string _edidFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "EDIDs");
        private bool _loading;
        private ObservableCollection<string> _edidFiles;
        private string _selectedFile; 
        #endregion

        #region Properties
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
        public ObservableCollection<string> EDIDFiles
        {
            get { return _edidFiles; }
            set
            {
                System.Diagnostics.Trace.WriteLine("EDID SET");
                if (_edidFiles == value)
                    return;

                _edidFiles = value;
                OnPropertyChanged("EDIDFiles");
            }
        }
        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (_selectedFile == value)
                    return;

                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
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
        private ICommand _importCommand;
        public ICommand ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new RelayCommand(HandleImportExecuted, HandleImportCanExecute));
            }
        }
        #endregion

        #region Methods
        private void HandleLoadedExecuted(object obj)
        {
            if (Loading)
                return;

            Loading = true;
            Task.Factory.StartNew(() =>
            {
                if (!Directory.Exists(_edidFolder))
                    Directory.CreateDirectory(_edidFolder);

                var files = new DirectoryInfo(_edidFolder).GetFiles().Where(fi => EDID.IsEDIDFile(fi.FullName)).Select(fi => fi.Name);
                App.Current.Dispatcher.BeginInvoke(new Action(() => { EDIDFiles = new ObservableCollection<string>(files); }), null);
                App.Current.Dispatcher.BeginInvoke(new Action(() => { Loading = false; }), null);
            });
        }

        private bool HandleLoadedCanExecute(object obj)
        {
            return !Loading;
        }

        private void HandleAcceptExecuted(object obj)
        {
            Messenger<Prompts>.Invoke(Prompts.None);

            if (SelectedFile == null)
                return;

            EDID edid = EDID.FromFile(Path.Combine(_edidFolder, SelectedFile));
            if (edid == null)
                return;

            IoC.Get<EDIDViewModel>().EDID = edid;
            IoC.Get<EDIDViewModel>().Filename = SelectedFile;
        }

        private bool HandleAcceptCanExecute(object obj)
        {
            return !String.IsNullOrEmpty(SelectedFile);
        }

        private void HandleCancelExecuted(object obj)
        {
            Messenger<Prompts>.Invoke(Prompts.None);
        }

        private bool HandleCancelCanExecute(object obj)
        {
            return true;
        } 

        private void HandleImportExecuted(object obj)
        {
            // For now fall back to standard windows dialoges; might be replaced with custom dialogs later on
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = LocalString("OFD_Title"),
                InitialDirectory = _edidFolder,
                Multiselect = false
            };
            string filter = LocalString("OFD_Filter");
            try
            {
                filter = filter.Trim();
                ofd.Filter = filter;
            }
            catch (Exception ex) { }
            if (ofd.ShowDialog() == true)
            {
                if (!EDID.IsEDIDFile(ofd.FileName))
                {
                    MessageBox.Show(LocalString("OFD_NoEDID"), LocalString("Error"), MessageBoxButton.OK);
                    return;
                }

                string destPath = Path.Combine(_edidFolder, Path.GetFileName(ofd.FileName));
                if (File.Exists(destPath))
                {
                    var replace = MessageBox.Show(LocalString("OFD_Exists"), LocalString("Error"), MessageBoxButton.YesNoCancel);
                    if (replace == MessageBoxResult.Cancel)
                        return;
                    if (replace == MessageBoxResult.No)
                        destPath = destPath.GetNextFilename();
                }
                File.Copy(ofd.FileName, destPath, true);

                if (!_edidFiles.Contains(Path.GetFileName(destPath)))
                    _edidFiles.Add(Path.GetFileName(destPath));
            }
        }
        
        private bool HandleImportCanExecute(object obj)
        {
            return true;
        }
        #endregion
    }
}
