using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace SquirrelEDID.Model
{
    public class TreeItem : NotifyPropertyChanged
    {
        #region Fields
        private bool _forbidden;
        private bool _isFullyLoaded;
        private string _name;
        private TreeItem _parent;
        private ObservableCollection<TreeItem> _children; 
        #endregion

        #region Properties
        public bool Forbidden
        {
            get { return _forbidden; }
            set
            {
                if (_forbidden == value)
                    return;

                _forbidden = value;
                OnPropertyChanged("Forbidden");
            }
        }
        public bool IsFullyLoaded
        {
            get { return _isFullyLoaded; }
            set
            {
                if (_isFullyLoaded == value)
                    return;

                _isFullyLoaded = value;
                OnPropertyChanged("IsFullyLoaded");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public TreeItem Parent
        {
            get { return _parent; }
            set
            {
                if (_parent == value)
                    return;

                _parent = value;
                OnPropertyChanged("Parent");
            }
        }
        public ObservableCollection<TreeItem> Children
        {
            get { return _children; }
            set
            {
                if (_children == value)
                    return;

                _children = value;
                OnPropertyChanged("Children");
            }
        } 
        #endregion

        #region Constructors
        public TreeItem(string name, TreeItem parent)
        {
            Name = name;
            IsFullyLoaded = false;
            Parent = parent;
            Children = new ObservableCollection<TreeItem>();
        } 
        #endregion

        #region Methods
        public string GetFullPath()
        {
            Stack<string> stack = new Stack<string>();

            var ti = this;
            while (ti.Parent != null)
            {
                stack.Push(ti.Name);
                ti = ti.Parent;
            }

            string path = stack.Pop();
            while (stack.Count > 0)
                path = Path.Combine(path, stack.Pop());

            return path;
        } 
        #endregion
    }

    public class DriveTreeItem : TreeItem
    {
        #region Fields
        private DriveType _driveType; 
        #endregion

        #region Properties
        public DriveType DriveType
        {
            get { return _driveType; }
            set
            {
                if (_driveType == value)
                    return;

                _driveType = value;
                OnPropertyChanged("DriveType");
            }
        } 
        #endregion

        #region Constructors
        public DriveTreeItem(string name, DriveType type, TreeItem parent)
            : base(name, parent)
        {
            DriveType = type;
        }
        #endregion
    }
}
