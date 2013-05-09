using SquirrelEDID.Model;
using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SquirrelEDID.Utilities.Converters
{
    public class FSItemToIconConverter : IValueConverter
    {
        public ImageSource Removable { get; set; }
        public ImageSource Drive { get; set; }
        public ImageSource NetDrive { get; set; }
        public ImageSource CDRom { get; set; }
        public ImageSource RAM { get; set; }
        public ImageSource Folder { get; set; }
        public ImageSource Forbidden { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is TreeItem))
                return null;

            TreeItem ti = (TreeItem)value;
            if(ti.Forbidden)
                return Forbidden;

            if (ti is DriveTreeItem)
            {
                DriveTreeItem dti = (DriveTreeItem)ti;
                switch (dti.DriveType)
                {
                    case System.IO.DriveType.CDRom:
                        return CDRom;
                    case System.IO.DriveType.Fixed:
                        return Drive;
                    case System.IO.DriveType.Network:
                        return NetDrive;
                    case System.IO.DriveType.NoRootDirectory:
                        return Drive;
                    case System.IO.DriveType.Ram:
                        return RAM;
                    case System.IO.DriveType.Removable:
                        return Removable;
                    case System.IO.DriveType.Unknown:
                        return Drive;
                }
            }
            else
                return Folder;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
