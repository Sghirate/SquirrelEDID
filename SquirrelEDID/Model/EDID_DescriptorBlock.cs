using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquirrelEDID.Utilities.Extensions;

namespace SquirrelEDID.Model
{
    //public partial class EDID : NotifyPropertyChanged
    //{
    //    public class DescriptorBlock : Descriptor
    //    {
    //        #region Fields
    //        private int _type;
    //        #endregion

    //        #region Properties
    //        public bool DetailedTimingDescriptor
    //        {
    //            get
    //            {
    //                return _parent.Buffer[_firstByte] > 0;
    //            }
    //            set
    //            {
    //                if (value)
    //                {
    //                    _parent.Buffer[_firstByte] = 1;
    //                }
    //                else
    //                {
    //                    _parent.Buffer[_firstByte] = 0;
    //                    ApplyType();
    //                }
    //                OnPropertyChanged("DetailedTimingDescriptor");
    //            }
    //        }
    //        public int Type
    //        {
    //            get
    //            {
    //                if (!DetailedTimingDescriptor)
    //                {
    //                    _type = _parent.Buffer[_firstByte + 3];
    //                    return _type;
    //                }
    //                else
    //                {
    //                    return 0;
    //                }
    //            }
    //            set
    //            {
    //                _type = value;
    //                ApplyType();

    //                OnPropertyChanged("Type");
    //            }
    //        }
    //        public string StringData
    //        {
    //            get
    //            {
    //                return _parent.Buffer.GetNullTermString(_firstByte + 5, 13);
    //            }
    //            set
    //            {
    //                int c = 0;
    //                int i = 0;
    //                bool ended = false;
    //                while (i < 13)
    //                {
    //                    if (c < value.Length - 1)
    //                    {
    //                        if (value[c] >= '0' && value[c] <= '~')
    //                        {
    //                            _parent.Buffer[_firstByte + i] = (byte)value[c];
    //                            c++;
    //                            i++;
    //                        }
    //                        else
    //                        {
    //                            c++;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (!ended)
    //                        {
    //                            _parent.Buffer[_firstByte + i] = 0x0a;
    //                            ended = true;
    //                        }
    //                        else
    //                        {
    //                            _parent.Buffer[_firstByte + i] = 0x20;
    //                        }
    //                        i++;
    //                    }
    //                }

    //                _parent.Validify();
    //                OnPropertyChanged("StringData");
    //            }
    //        }
    //        public DetailedTimingDescriptor DTD { get; set; }
    //        public MonitorRangeLimitsDescriptor RNG { get; set; }
    //        public CustomTiming[] TimingDescriptors { get; set; }
    //        public AdditionalWhitePointDescriptor WPD { get; set; }
    //        #endregion

    //        #region Constructors
    //        public DescriptorBlock(EDID parent, int firstByte)
    //            : base(parent, firstByte)
    //        {
    //            TimingDescriptors = new CustomTiming[6];
    //            for (int i = 0; i < 6; i++)
    //                TimingDescriptors[i] = new CustomTiming(_parent, _firstByte + 5 + i * 2);
    //            DTD = new DetailedTimingDescriptor(parent, firstByte);
    //            RNG = new MonitorRangeLimitsDescriptor(parent, firstByte);
    //            WPD = new AdditionalWhitePointDescriptor(parent, firstByte);
    //        }
    //        #endregion

    //        #region Methods
    //        private void ApplyType()
    //        {
    //            if (DetailedTimingDescriptor)
    //                return;

    //            _parent.Buffer[_firstByte] = 0;
    //            _parent.Buffer[_firstByte + 1] = 0;
    //            _parent.Buffer[_firstByte + 2] = 0;
    //            _parent.Buffer[_firstByte + 3] = (byte)(_type & 0xff);
    //            _parent.Buffer[_firstByte + 4] = 0;
    //            switch (_type)
    //            {
    //                case 0xfa: // Additional Standard Timing Descriptor
    //                    _parent.Buffer[_firstByte + 17] = 0x0a; // padding
    //                    break;
    //                case 0xfb: // Additional White Point Descriptor
    //                    _parent.Buffer[_firstByte + 17] = 0x0a; // padding
    //                    _parent.Buffer[_firstByte + 17] = 0x20; // padding
    //                    _parent.Buffer[_firstByte + 17] = 0x20; // padding
    //                    break;
    //            }
    //        }
    //        #endregion
    //    }
    //}
}
