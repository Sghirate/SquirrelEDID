
namespace SquirrelEDID.Model
{
    public partial class EDID : NotifyPropertyChanged
    {
        public class AdditionalWhitePointDescriptor : Descriptor
        {
            #region Fields
            private WhitePointDescriptor[] _whitePointDescriptors; 
            #endregion

            #region Properties
            public WhitePointDescriptor[] WhitePointDescriptors
            {
                get
                {
                    return _whitePointDescriptors;
                }
            } 
            #endregion

            #region Constructors
            public AdditionalWhitePointDescriptor(EDID parent, int firstByte) : base(parent, firstByte, 0xfb) 
            {
                _whitePointDescriptors = new WhitePointDescriptor[2];
                _whitePointDescriptors[1] = new WhitePointDescriptor(parent, firstByte + 5);
                _whitePointDescriptors[2] = new WhitePointDescriptor(parent, firstByte + 10);
            }
            #endregion
                
            #region Methods
            protected override void Apply()
            {
                base.Apply();

                _parent.Buffer[15] = 0x0a;
                _parent.Buffer[16] = 0x20;
                _parent.Buffer[17] = 0x20;

                _parent.Validify();
            }
            #endregion

            public class WhitePointDescriptor : NotifyPropertyChanged
            {
                #region Fields
                private EDID _parent;
                private int _firstByte; 
                #endregion

                #region Properties
                public bool Used
                {
                    get
                    {
                        return _parent.Buffer[_firstByte] == 1;
                    }
                    set
                    {
                        _parent.Buffer[_firstByte] = (byte)((value ? 1 : 0) & 0xff);
                        _parent.Validify();

                        OnPropertyChanged("Used");
                    }
                } 
                public int WhiteX
                {
                    get
                    {
                        return _parent.GetColor(_firstByte + 2, _firstByte + 1, 2);
                    }
                    set
                    {
                        _parent.SetColor(_firstByte + 2, _firstByte + 1, 2, value);
                        _parent.Validify();

                        OnPropertyChanged("WhiteX");
                    }
                }
                public double ClrWhiteX
                {
                    get
                    {
                        return (double)WhiteX / 1024.0;
                    }
                    set
                    {
                        int v = (int)(value * 1024.0);
                        if (v > 1023)
                            v = 1023;
                        else if (v < 0)
                            v = 0;
                        WhiteX = v;

                        OnPropertyChanged("ClrWhiteX");
                    }
                }
                public int WhiteY
                {
                    get
                    {
                        return _parent.GetColor(_firstByte + 3, _firstByte + 1, 0);
                    }
                    set
                    {
                        _parent.SetColor(_firstByte + 3, _firstByte + 1, 0, value);
                        _parent.Validify();

                        OnPropertyChanged("WhiteY");
                    }
                }
                public double ClrWhiteY
                {
                    get
                    {
                        return (double)WhiteX / 1024.0;
                    }
                    set
                    {
                        int v = (int)(value * 1024.0);
                        if (v > 1023)
                            v = 1023;
                        else if (v < 0)
                            v = 0;
                        WhiteX = v;

                        OnPropertyChanged("ClrWhiteY");
                    }
                }
                public int Gamma
                {
                    get
                    {
                        return (_parent.Buffer[_firstByte + 4] * 100) - 100;
                    }
                    set
                    {
                        _parent.Buffer[_firstByte + 4] = (byte)(((value + 100) / 100) & 0xff);

                        _parent.Validify();
                        OnPropertyChanged("Gamma");
                    }
                }
                #endregion

                #region Constructors
                public WhitePointDescriptor(EDID parent, int firstByte)
                {
                    _parent = parent;
                    _firstByte = firstByte;

                    _parent.SetBit(_firstByte + 1, 7, false);
                    _parent.SetBit(_firstByte + 1, 6, false);
                    _parent.SetBit(_firstByte + 1, 5, false);
                    _parent.SetBit(_firstByte + 1, 4, false);
                } 
                #endregion
            }
        }
    }
}
