using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquirrelEDID.Model
{
    public partial class EDID : NotifyPropertyChanged
    {
        public class MonitorRangeLimitsDescriptor : Descriptor
        {
            #region Properties
            public int MinVerticalField
            {
                get
                {
                    return _parent.Buffer[_firstByte + 5];
                }
                set
                {
                    _parent.Buffer[_firstByte + 5] = (byte)(value & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("MinVerticalField");
                }
            }
            public int MaxVerticalField
            {
                get
                {
                    return _parent.Buffer[_firstByte + 6];
                }
                set
                {
                    _parent.Buffer[_firstByte + 6] = (byte)(value & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("MaxVerticalField");
                }
            }
            public int MinHorizontalLine
            {
                get
                {
                    return _parent.Buffer[_firstByte + 7];
                }
                set
                {
                    _parent.Buffer[_firstByte + 8] = (byte)(value & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("MinHorizontalLine");
                }
            }
            public int MaxHorizontalLine
            {
                get
                {
                    return _parent.Buffer[_firstByte + 8];
                }
                set
                {
                    _parent.Buffer[_firstByte + 8] = (byte)(value & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("MaxHorizontalLine");
                }
            }
            public int MaxPixelClock
            {
                get
                {
                    return _parent.Buffer[_firstByte + 9];
                }
                set
                {
                    _parent.Buffer[_firstByte + 9] = (byte)(value & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("MaxPixelClock");
                }
            }
            public bool ExtendedTimingInfo
            {
                get
                {
                    return _parent.Buffer[_firstByte + 10] == 0x02;
                }
                set
                {
                    if (value && _parent.Buffer[_firstByte + 10] == 0x0a)
                    {
                        _parent.Buffer[_firstByte + 10] = 0x02;
                        for (int i = 11; i < 18; i++)
                            _parent.Buffer[_firstByte + i] = 0x00;
                    }
                    else if (!value && _parent.Buffer[_firstByte + 10] == 0x02)
                    {
                        _parent.Buffer[_firstByte + 10] = 0x0a;
                        for (int i = 11; i < 18; i++)
                            _parent.Buffer[_firstByte + i] = 0x20;
                    }

                    _parent.Validify();
                    OnPropertyChanged("ExtendedTimingInfo");
                }
            }
            public int StartFreq
            {
                get
                {
                    return _parent.Buffer[_firstByte + 12] * 10;
                }
                set
                {
                    _parent.Buffer[_firstByte + 12] = (byte)((value / 10) & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("StartFreq");
                }
            }
            public double GTF_C
            {
                get
                {
                    return (double)_parent.Buffer[_firstByte + 13] / 2.0;
                }
                set
                {
                    _parent.Buffer[_firstByte + 13] = (byte)(((int)value * 2) & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("GTF_C");
                }
            }
            public int GTF_M
            {
                get
                {
                    return (_parent.Buffer[_firstByte + 14] << 8) + _parent.Buffer[_firstByte + 15];
                }
                set
                {
                    _parent.Buffer[_firstByte + 14] = (byte)((value >> 8) & 0xff);
                    _parent.Buffer[_firstByte + 15] = (byte)(value & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("GTF_M");
                }
            }
            public int GTF_K
            {
                get
                {
                    return _parent.Buffer[_firstByte + 16];
                }
                set
                {
                    _parent.Buffer[_firstByte + 16] = (byte)(value & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("GTF_K");
                }
            }
            public double GTF_J
            {
                get
                {
                    return (double)_parent.Buffer[_firstByte + 17] / 2.0;
                }
                set
                {
                    _parent.Buffer[_firstByte + 17] = (byte)(((int)value * 2) & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("GTF_J");
                }
            }
            #endregion

            #region Constructors
            public MonitorRangeLimitsDescriptor(EDID parent, int firstByte) : base(parent, firstByte, 0xfd) { }
            #endregion
        }
    }
}
