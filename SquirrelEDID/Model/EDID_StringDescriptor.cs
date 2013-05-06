using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SquirrelEDID.Utilities.Extensions;

namespace SquirrelEDID.Model
{
    public partial class EDID : NotifyPropertyChanged
    {
        public class StringDescriptor : Descriptor
        {
            public string StringData
            {
                get
                {
                    return _parent.Buffer.GetNullTermString(_firstByte + 5, 13);
                }
                set
                {
                    int c = 0;
                    int i = 0;
                    bool ended = false;
                    while (i < 13)
                    {
                        if (c < value.Length - 1)
                        {
                            if (value[c] >= '0' && value[c] <= '~')
                            {
                                _parent.Buffer[_firstByte + i] = (byte)value[c];
                                c++;
                                i++;
                            }
                            else
                            {
                                c++;
                            }
                        }
                        else
                        {
                            if (!ended)
                            {
                                _parent.Buffer[_firstByte + i] = 0x0a;
                                ended = true;
                            }
                            else
                            {
                                _parent.Buffer[_firstByte + i] = 0x20;
                            }
                            i++;
                        }
                    }

                    _parent.Validify();
                    OnPropertyChanged("StringData");
                }
            }

            #region Constructors
            public StringDescriptor(EDID parent, int firstByte, int type) : base(parent, firstByte, type) { } 
            #endregion
        }
    }
}
