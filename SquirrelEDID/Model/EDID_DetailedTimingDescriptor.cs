using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquirrelEDID.Model
{
    public partial class EDID : NotifyPropertyChanged
    {
        public class DetailedTimingDescriptor : Descriptor
        {
            #region Properties
            public int PixelClock
            {
                get
                {
                    int val = _parent.Buffer[_firstByte] << 8 + _parent.Buffer[_firstByte + 1];
                    return val * 10;
                }
                set
                {
                    int val = (int)(value / 10);
                    _parent.Buffer[_firstByte + 1] = (byte)(val & 0xff);
                    _parent.Buffer[_firstByte] = (byte)((val >> 8) & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("PixelClock");
                }
            }
            public int HorizontalActivePixels
            {
                get
                {
                    return _parent.Buffer[_firstByte + 2] + ((_parent.Buffer[_firstByte + 4] & 0xf0) << 4);
                }
                set
                {
                    _parent.Buffer[_firstByte + 2] = (byte)(value & 0xff);
                    _parent.Buffer[_firstByte + 4] = (byte)(_parent.Buffer[_firstByte + 4] & ((value >> 4) | ~0xf0));

                    _parent.Validify();
                    OnPropertyChanged("HorizontalActivePixels");
                }
            }
            public int HorizontalBlankingPixels
            {
                get
                {
                    return _parent.Buffer[_firstByte + 3] + ((_parent.Buffer[_firstByte + 4] & 0x0f) << 8);
                }
                set
                {
                    _parent.Buffer[_firstByte + 3] = (byte)(value & 0xff);
                    _parent.Buffer[_firstByte + 4] = (byte)(_parent.Buffer[_firstByte + 4] & ((value >> 8) | ~0x0f));

                    _parent.Validify();
                    OnPropertyChanged("HorizontalBlankingPixels");
                }
            }
            public int VerticalActiveLines
            {
                get
                {
                    return _parent.Buffer[_firstByte + 5] + ((_parent.Buffer[_firstByte + 7] & 0xf0) << 8);
                }
                set
                {
                    _parent.Buffer[_firstByte + 5] = (byte)(value & 0xff);
                    _parent.Buffer[_firstByte + 7] = (byte)(_parent.Buffer[_firstByte + 7] & ((value >> 4) | ~0xf0));

                    _parent.Validify();
                    OnPropertyChanged("VerticalActiveLines");
                }
            }
            public int VerticalBlankingLines
            {
                get
                {
                    return _parent.Buffer[_firstByte + 6] + ((_parent.Buffer[_firstByte + 7] & 0x0f) << 8);
                }
                set
                {
                    _parent.Buffer[_firstByte + 6] = (byte)(value & 0xff);
                    _parent.Buffer[_firstByte + 7] = (byte)(_parent.Buffer[_firstByte + 7] & ((value >> 8) | ~0x0f));

                    _parent.Validify();
                    OnPropertyChanged("VerticalBlankingLines");
                }
            }
            public int HorizontalSyncOffset
            {
                get
                {
                    return _parent.Buffer[_firstByte + 8] + ((_parent.Buffer[_firstByte + 11] & 0xc0) << 2);
                }
                set
                {
                    _parent.Buffer[_firstByte + 8] = (byte)(value & 0xff);
                    _parent.Buffer[_firstByte + 11] = (byte)(_parent.Buffer[_firstByte + 11] & ((value >> 2) | ~0xc0));

                    _parent.Validify();
                    OnPropertyChanged("HorizontalSyncOffset");
                }
            }
            public int HorizontalSyncPulse
            {
                get
                {
                    return _parent.Buffer[_firstByte + 9] + ((_parent.Buffer[_firstByte + 11] & 0x30) << 4);
                }
                set
                {
                    _parent.Buffer[_firstByte + 9] = (byte)(value & 0xff);
                    _parent.Buffer[_firstByte + 11] = (byte)(_parent.Buffer[_firstByte + 11] & ((value >> 4) | ~0x30));

                    _parent.Validify();
                    OnPropertyChanged("HorizontalSyncPulse");
                }
            }
            public int VerticalSyncOffset
            {
                get
                {
                    return ((_parent.Buffer[_firstByte + 10] & 0xf0) >> 4) + ((_parent.Buffer[_firstByte + 11] & 0x0c) << 2);
                }
                set
                {
                    _parent.Buffer[_firstByte + 10] = (byte)(_parent.Buffer[_firstByte + 10] & ((value << 4) | ~0xf0));
                    _parent.Buffer[_firstByte + 11] = (byte)(_parent.Buffer[_firstByte + 11] & ((value >> 2) | ~0x0c));

                    _parent.Validify();
                    OnPropertyChanged("VerticalSyncOffset");
                }
            }
            public int VerticalSyncPulse
            {
                get
                {
                    return (_parent.Buffer[_firstByte + 10] & 0x0f) + ((_parent.Buffer[_firstByte + 11] & 0x03) << 4);
                }
                set
                {
                    _parent.Buffer[_firstByte + 10] = (byte)(_parent.Buffer[_firstByte + 10] & (value | ~0x0f));
                    _parent.Buffer[_firstByte + 11] = (byte)(_parent.Buffer[_firstByte + 11] & ((value >> 4) | ~0x03));

                    _parent.Validify();
                    OnPropertyChanged("VerticalSyncPulse");
                }
            }
            public int HorizontalDisplaySize
            {
                get
                {
                    return _parent.Buffer[_firstByte + 12] + ((_parent.Buffer[_firstByte + 14] & 0xf0) << 4);
                }
                set
                {
                    _parent.Buffer[_firstByte + 12] = (byte)(value & 0xff);
                    _parent.Buffer[_firstByte + 14] = (byte)(_parent.Buffer[_firstByte + 14] & ((value >> 4) | ~0xf0));

                    _parent.Validify();
                    OnPropertyChanged("HorizontalDisplaySize");
                }
            }
            public int VerticalDisplaySize
            {
                get
                {
                    return _parent.Buffer[_firstByte + 13] + ((_parent.Buffer[_firstByte + 14] & 0x0f) << 8);
                }
                set
                {
                    _parent.Buffer[_firstByte + 13] = (byte)(value & 0xff);
                    _parent.Buffer[_firstByte + 14] = (byte)(_parent.Buffer[_firstByte + 14] & ((value >> 8) | ~0x0f));

                    _parent.Validify();
                    OnPropertyChanged("VerticalDisplaySize");
                }
            }
            public int HorizontalBorderPixels
            {
                get
                {
                    return _parent.Buffer[_firstByte + 15];
                }
                set
                {
                    _parent.Buffer[_firstByte + 15] = (byte)(value & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("HorizontalBorderPixels");
                }
            }
            public int VerticalBorderPixels
            {
                get
                {
                    return _parent.Buffer[_firstByte + 16];
                }
                set
                {
                    _parent.Buffer[_firstByte + 16] = (byte)(value & 0xff);

                    _parent.Validify();
                    OnPropertyChanged("VerticalBorderPixels");
                }
            }
            public bool Interlaced
            {
                get
                {
                    return _parent.GetBit(_firstByte + 17, 7);
                }
                set
                {
                    _parent.SetBit(_firstByte + 17, 7, value);

                    _parent.Validify();
                    OnPropertyChanged("Interlaced");
                }
            }
            public int StereoMode
            {
                get
                {
                    return (_parent.Buffer[_firstByte + 17] & 0x60) >> 5;
                }
                set
                {
                    _parent.Buffer[_firstByte + 17] = (byte)(_parent.Buffer[_firstByte + 17] & ((value << 5) | ~0x60));

                    _parent.Validify();
                    OnPropertyChanged("StereoMode");
                }
            }
            public int SyncType
            {
                get
                {
                    return (_parent.Buffer[_firstByte + 17] & 0x18) >> 3;
                }
                set
                {
                    _parent.Buffer[_firstByte + 17] = (byte)(_parent.Buffer[_firstByte + 17] & ((value << 3) | ~0x18));

                    _parent.Validify();
                    OnPropertyChanged("SyncType");
                }
            }
            public bool VSync
            {
                get
                {
                    return _parent.GetBit(_firstByte + 17, 2);
                }
                set
                {
                    _parent.SetBit(_firstByte + 17, 2, value);

                    _parent.Validify();
                    OnPropertyChanged("VSync");
                }
            }
            public bool HSync
            {
                get
                {
                    return _parent.GetBit(_firstByte + 17, 1);
                }
                set
                {
                    _parent.SetBit(_firstByte + 17, 1, value);

                    _parent.Validify();
                    OnPropertyChanged("HSync");
                }
            }
            public bool InterleavedStereo
            {
                get
                {
                    return _parent.GetBit(_firstByte + 17, 0);
                }
                set
                {
                    _parent.SetBit(_firstByte + 17, 0, value);

                    _parent.Validify();
                    OnPropertyChanged("InterleavedStereo");
                }
            }
            #endregion

            #region Constructors
            public DetailedTimingDescriptor(EDID parent, int firstByte) : base(parent, firstByte, 0) 
            { 
            }
            #endregion
        }
    }
}
