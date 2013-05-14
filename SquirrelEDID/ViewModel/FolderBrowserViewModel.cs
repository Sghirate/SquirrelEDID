using SquirrelEDID.Model;
using SquirrelEDID.Resources;
using SquirrelEDID.Utilities;
using SquirrelEDID.Utilities.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Input;

namespace SquirrelEDID.ViewModel
{
    public class FolderBrowserViewModel : BaseViewModel
    {
        #region Fields
        private TreeItem _root;
        private TreeItem _selectedItem;
        private string _initialPath;
        private string _selectedPath;
        private string _emptyName;
        private string _newFolderName; 
        #endregion

        #region Properties
        public TreeItem Root
        {
            get { return _root; }
            set
            {
                if (_root == value)
                    return;

                _root = value;
                OnPropertyChanged("Root");
            }
        }
        public TreeItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        public string InitialPath
        {
            get { return _initialPath; }
            set
            {
                if (_initialPath == value)
                    return;

                _initialPath = value;
                OnPropertyChanged("InitialPath");
            }
        }
        public string SelectedPath
        {
            get { return _selectedPath; }
            set
            {
                if (_selectedPath == value)
                    return;

                _selectedPath = value;
                OnPropertyChanged("SelectedPath");
            }
        } 
        #endregion

        #region Commands
        private ICommand _selectCommand;
        public ICommand SelectCommand
        {
            get
            {
                return _selectCommand ?? (_selectCommand = new RelayCommand(HandleSelectExecuted));
            }
        }
        private ICommand _expandCommand;
        public ICommand ExpandCommand
        {
            get
            {
                return _expandCommand ?? (_expandCommand = new RelayCommand(HandleExpandExecuted));
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

        #region Constructors
        public FolderBrowserViewModel()
        {
            _newFolderName = Strings.FolderBrowser_NewFolder;
            _emptyName = Strings.FolderBrowser_Empty;
            Init();
        } 
        #endregion

        #region Methods
        private void Init()
        {
            TreeItem root = new TreeItem("root", null);

            foreach (DriveInfo di in DriveInfo.GetDrives())
            {
                TreeItem item = new DriveTreeItem(di.Name, di.DriveType, root);
                item.Children.Add(new TreeItem(_emptyName, item));

                root.Children.Add(item);
            }

            Root = root;
        }

        private void HandleSelectExecuted(object obj)
        {
            TreeItem ti = (TreeItem)obj;
            if (ti == null)
                return;

            SelectedItem = ti;
            CheckSelected();
        }

        private void HandleExpandExecuted(object obj)
        {
            TreeItem ti = (TreeItem)obj;
            if (ti == null)
                return;

            if (ti.IsFullyLoaded)
                return;

            ti.Children.Clear();
            string path = ti.GetFullPath();
            DirectoryInfo di = new DirectoryInfo(path);
            try
            {
                foreach (DirectoryInfo sub in di.GetDirectories())
                {
                    TreeItem item = new TreeItem(sub.Name, ti);
                    item.Children.Add(new TreeItem(_emptyName, item));

                    ti.Children.Add(item);
                }
            }
            catch(UnauthorizedAccessException ex) 
            {
                ti.Forbidden = true;
                CheckSelected();
            }
            ti.IsFullyLoaded = true;
        }

        private void CheckSelected()
        {
            if (!SelectedItem.Forbidden && !SelectedItem.IsFullyLoaded)
            {
                try
                {
                    new DirectoryInfo(SelectedItem.GetFullPath()).GetDirectories();
                }
                catch (UnauthorizedAccessException ex)
                {
                    SelectedItem.Forbidden = true;
                }
            }
            if (SelectedItem.Forbidden)
                SelectedPath = null;
            else
                SelectedPath = SelectedItem.GetFullPath();
        }
        
        private void HandleCancelExecuted(object obj)
        {
        }

        private bool HandleCancelCanExecute(object obj)
        {
            return true;
        }

        private void HandleAcceptExecuted(object obj)
        {
        }

        private bool HandleAcceptCanExecute(object obj)
        {
            return Directory.Exists(SelectedPath);
        }
        #endregion
    }
}
