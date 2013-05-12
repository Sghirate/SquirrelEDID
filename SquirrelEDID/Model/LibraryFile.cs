using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquirrelEDID.Model
{
    public class LibraryEntry : NotifyPropertyChanged
    {
        private string _path;
        private string _name;
        private EDID _edid;

        public string Path
        {
            get { return _path; }
            set
            {
                if (_path == value)
                    return;

                _path = value;
                OnPropertyChanged("Path");
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
    }
}
