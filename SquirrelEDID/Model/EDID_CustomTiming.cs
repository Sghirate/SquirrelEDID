
namespace SquirrelEDID.Model
{
    public partial class EDID : NotifyPropertyChanged
    {
        public class CustomTiming : Descriptor
        {
            #region Properties
            public bool Used
            {
                get
                {
                    if (_parent.Version == 1 && _parent.Revision < 3)
                        return !(_parent.Buffer[_firstByte] == 1 && _parent.Buffer[_firstByte + 1] == 0);
                    else
                        return !(_parent.Buffer[_firstByte] == 1 && _parent.Buffer[_firstByte + 1] == 1);
                }
                set
                {
                    if (value)
                    {
                        if (_parent.Version == 1 && _parent.Revision < 3)
                        {
                            if (_parent.Buffer[_firstByte + 1] == 0)
                                _parent.Buffer[_firstByte + 1] = 1;
                        }
                        else
                        {
                            if (_parent.Buffer[_firstByte + 1] == 1)
                                _parent.Buffer[_firstByte + 1] = 0;
                        }
                    }
                    else
                    {
                        if (_parent.Version == 1 && _parent.Revision < 3)
                            _parent.Buffer[_firstByte] = _parent.Buffer[_firstByte + 1] = 0;
                        else
                            _parent.Buffer[_firstByte] = _parent.Buffer[_firstByte + 1] = 1;
                    }

                    _parent.Validify();
                    OnPropertyChanged("Used");
                    OnPropertyChanged("XResolution");
                    OnPropertyChanged("AspectRatio");
                    OnPropertyChanged("VerticalFrequency");
                    _parent.OnPropertyChanged("Buffer");
                }
            }
            public int XResolution
            {
                get
                {
                    if (!Used)
                        return 0;

                    return (_parent.Buffer[_firstByte] + 31) * 8;
                }
                set
                {
                    if (value < 256)
                    {
                        _parent.Buffer[_firstByte] = 0;
                    }
                    else
                    {
                        _parent.Buffer[_firstByte] = (byte)((int)((value / 8) - 31) & 0xff);
                    }

                    _parent.Validify();
                    OnPropertyChanged("XResolution");
                    OnPropertyChanged("Used");
                    _parent.OnPropertyChanged("Buffer");
                }
            }
            public int AspectRatio
            {
                get
                {
                    return _parent.Buffer[_firstByte + 1] >> 6;
                }
                set
                {
                    _parent.Buffer[_firstByte + 1] = (byte)((_parent.Buffer[_firstByte + 1] & ((value << 6) | 0x3f)) & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("AspectRatio");
                    OnPropertyChanged("Used");
                    _parent.OnPropertyChanged("Buffer");
                }
            }
            public int VerticalFrequency
            {
                get
                {
                    if (!Used)
                        return 0;

                    return (_parent.Buffer[_firstByte + 1] & 0x3f) + 60;
                }
                set
                {
                    _parent.Buffer[_firstByte + 1] = (byte)(_parent.Buffer[_firstByte + 1] & ((value & 0x3f) | 0xc0));

                    _parent.Validify();
                    OnPropertyChanged("VerticalFrequency");
                    OnPropertyChanged("Used");
                    _parent.OnPropertyChanged("Buffer");
                }
            }
            #endregion

            #region Constructors
            public CustomTiming(EDID parent, int firstByte) : base(parent, firstByte, 0xfa)
            {
            }
            #endregion

            #region Methods
            protected override void Apply()
            {
                //base.Apply();
                //_parent.Buffer[17] = 0x0a;
                _parent.Validify();
            }
            #endregion
        }
    }
}
