
namespace SquirrelEDID.Model
{
    public partial class EDID : NotifyPropertyChanged
    {
        public class CustomTiming : Descriptor
        {
            #region Fields
            private int _xResolution;
            private int _aspectRatio;
            private int _verticalFrequency;
            #endregion

            #region Properties
            public string[] AspectRatios { get; set; }
            public bool Used
            {
                get
                {
                    return _parent.Buffer[_firstByte] != 0x01 || _parent.Buffer[_firstByte + 1] != 0x01;
                }
                set
                {
                    if (value)
                    {
                        SetResolution();
                        SetAspectRatio();
                        SetVerticalFrequency();
                    }
                    else
                    {
                        _parent.Buffer[_firstByte] = 0x01;
                        _parent.Buffer[_firstByte + 1] = 0x01;
                        _parent.Validify();
                    }

                    OnPropertyChanged("Used");
                }
            }
            public int XResolution
            {
                get
                {
                    if (Used)
                        GetResolution();
                    return _xResolution;
                }
                set
                {
                    _xResolution = value;
                    if (Used)
                        SetResolution();

                    OnPropertyChanged("XResolution");
                }
            }
            public int AspectRatio
            {
                get
                {
                    if (Used)
                        GetAspectRatio();

                    return _aspectRatio;
                }
                set
                {
                    _aspectRatio = value;
                    if (Used)
                        SetAspectRatio();

                    OnPropertyChanged("AspectRatio");
                }
            }
            public int VerticalFrequency
            {
                get
                {
                    if (Used)
                        GetVerticalFrequency();

                    return _verticalFrequency;
                }
                set
                {
                    _verticalFrequency = value;
                    if (Used)
                        SetVerticalFrequency();

                    OnPropertyChanged("VerticalFrequency");
                }
            }
            #endregion

            #region Constructors
            public CustomTiming(EDID parent, int firstByte) : base(parent, firstByte, 0xfa)
            {
                AspectRatios = _parent.AspectRatios;

                GetResolution();
                GetAspectRatio();
                GetVerticalFrequency();
            }
            #endregion

            #region Methods
            protected override void Apply()
            {
                base.Apply();

                _parent.Buffer[17] = 0x0a;

                _parent.Validify();
            }

            private void SetResolution()
            {
                int res = (_xResolution / 8) - 31;
                if (res <= 0)
                    res = 0;
                _parent.Buffer[_firstByte] = (byte)(res & 0xff);

                _parent.Validify();
            }

            private void GetResolution()
            {
                _xResolution = (_parent.Buffer[_firstByte] + 31) * 8;
            }

            private void SetAspectRatio()
            {
                if (_aspectRatio < 0)
                    _aspectRatio = 0;
                else if (_aspectRatio > 3)
                    _aspectRatio = 3;

                _parent.Buffer[_firstByte + 1] = (byte)((_parent.Buffer[_firstByte + 1] & ((_aspectRatio << 6) | 0x3f)) & 0xff);

                _parent.Validify();
            }

            private void GetAspectRatio()
            {
                _aspectRatio = (_parent.Buffer[_firstByte + 1] >> 6);
            }

            private void SetVerticalFrequency()
            {
                if (_verticalFrequency < 60)
                    _verticalFrequency = 60;
                else if (_verticalFrequency > 123)
                    _verticalFrequency = 123;

                int val = _verticalFrequency - 60;

                _parent.Buffer[_firstByte + 1] = (byte)(_parent.Buffer[_firstByte + 1] & ((val & 0x3f) | 0xc0));

                _parent.Validify();
            }

            private void GetVerticalFrequency()
            {
                _verticalFrequency = (_parent.Buffer[_firstByte + 1] & 0x3f) + 60;
            }
            #endregion
        }
    }
}
