
namespace SquirrelEDID.Model
{
    public partial class EDID : NotifyPropertyChanged
    {
        public class Descriptor : NotifyPropertyChanged
        {
            #region Fields
            protected EDID _parent;
            protected int _firstByte;
            protected int _type;
            #endregion

            #region Properties
            public int Type
            {
                get
                {
                    if (_type > 0)
                    {
                        _type = _parent.Buffer[_firstByte + 3];
                        return _type;
                    }
                    else
                    {
                        return 0;
                    }
                }
                set
                {
                    _type = value;
                    Apply();

                    OnPropertyChanged("Type");
                }
            } 
            #endregion

            #region Constructors
            public Descriptor(EDID parent, int firstByte, int type)
            {
                _parent = parent;
                _firstByte = firstByte;
                _type = type;
                if (_type > 0)
                    Apply();
            }
            #endregion

            #region Methods
            protected virtual void Apply()
            {
                _parent.Buffer[_firstByte] = 0;
                _parent.Buffer[_firstByte + 1] = 0;
                _parent.Buffer[_firstByte + 2] = 0;
                _parent.Buffer[_firstByte + 3] = (byte)(_type & 0xff);
                _parent.Buffer[_firstByte + 4] = 0;

                _parent.Validify();
            }

            public static Descriptor GetDescriptor(EDID parent, int firstByte)
            {
                if (parent.Buffer[firstByte] > 0)
                    return new DetailedTimingDescriptor(parent, firstByte);
                else
                    switch (parent.Buffer[firstByte + 3])
                    {
                        case 0xff:
                            return new StringDescriptor(parent, firstByte, 0xff);
                        case 0xfe:
                            return new StringDescriptor(parent, firstByte, 0xfe);
                        case 0xfc:
                            return new StringDescriptor(parent, firstByte, 0xfc);
                        case 0xfd:
                            return new MonitorRangeLimitsDescriptor(parent, firstByte);
                        case 0xfb:
                            return new AdditionalWhitePointDescriptor(parent, firstByte);
                        case 0xfa:
                            return new CustomTiming(parent, firstByte);
                    };
                return new Descriptor(parent, firstByte, 0);
            }
            #endregion
        }
    }
}
