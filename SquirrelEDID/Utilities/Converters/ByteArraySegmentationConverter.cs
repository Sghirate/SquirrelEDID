using System;
using System.Windows.Data;

namespace SquirrelEDID.Utilities.Converters
{
    public class ByteArraySegmentationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is byte[]))
                return null;

            byte[] buffer = (byte[])value;
            ByteArrayPiece[] segments = new ByteArrayPiece[8];
            for (int i = 0; i < 8; i++)
                segments[i] = new ByteArrayPiece(buffer, i * 16, 16);

            return segments;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public struct ByteArrayPiece
    {
        #region Fields
        private byte[] _data;
        private int _offset;
        private int _length; 
        #endregion

        #region Properties
        public int Offset
        {
            get
            {
                return _offset;
            }
        }
        public byte this[int index]
        {
            get
            {
                return _length > index ? _data[_offset + index] : (byte)0;
            }

            set
            {
                if (_length > index)
                    _data[_offset + index] = value;
            }
        } 
        #endregion

        #region Constructors
        public ByteArrayPiece(byte[] array, int offset, int length)
            : this()
        {
            _data = array;
            _offset = offset;
            _length = length;
        } 
        #endregion
    }
}
