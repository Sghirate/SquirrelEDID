using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SquirrelEDID.Utilities.Extensions;
using System.Collections;
using System.ComponentModel;

namespace SquirrelEDID.Model
{
    public partial class EDID : NotifyPropertyChanged
    {
        #region Fields
        private static byte[] PATTERNS = new byte[] { 0xff, 0xfe, 0xfc };
        private static byte[] HEADER = new byte[] { 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00 };
        private static string HEADER_TXT = "00ffffffffffff00";
        private static string HEADER_DAT = "Eyevis DDC Programmer dat file";
        private int _serialStart = -1;
        private byte _serialType = 0;
        private string _serial = null;
        private byte[] _buffer;
        private static string[] _displayTypesDigital = new string[]
        {
            "RGB 4:4:4",
            "RGB 4:4:4 + YCrCb 4:4:4",
            "RGB 4:4:4 + YCrCb 4:2:2",
            "RGB 4:4:4 + YCrCb 4:4:4 + YCrCb 4:2:2"
        };
        private static string[] _displayTypesAnalog = new string[]
        {
            "Monochrome or Grayscale",
            "RGB color",
            "Non-RGB color",
            "Undefined"
        };
        private static string[] _whiteAndSyncLevels = new string[] 
        { 
            "+0.7/−0.3 V",
            "+0.714/−0.286 V",
            "+1.0/−0.4 V",
            "+0.7/0 V"
        };
        private static string[] _aspectRatios = new string[]
        {
            "16:10",
            "4:3",
            "5:4",
            "16:9"
        };
        private static Dictionary<int, string> _establishedTimings = new Dictionary<int, string>
        {
            { 0,  "800×600 @ 60 Hz" },
            { 1,  "800×600 @ 56 Hz" },
            { 2,  "640×480 @ 75 Hz" },
            { 3,  "640×480 @ 72 Hz" },
            { 4,  "640×480 @ 67 Hz" },
            { 5,  "640×480 @ 60 Hz" },
            { 6,  "720×400 @ 88 Hz" },
            { 7,  "720×400 @ 70 Hz" },
            { 8,  "1280×1024 @ 75 Hz" },
            { 9,  "1024×768 @ 75 Hz" },
            { 10, "1024×768 @ 72 Hz" },
            { 11, "1024×768 @ 60 Hz" },
            { 12, "1024×768 @ 87 Hz, interlaced (1024×768i)" },
            { 13, "832×624 @ 75 Hz" },
            { 14, "800×600 @ 75 Hz" },
            { 15, "800×600 @ 72 Hz" },
            { 16, "Manufacturer VII" },
            { 17, "Manufacturer VI" },
            { 18, "Manufacturer V" },
            { 19, "Manufacturer IV" },
            { 20, "Manufacturer III" },
            { 21, "Manufacturer II" },
            { 22, "Manufacturer I" },
            { 23, "1152x870 @ 75 Hz (Apple Macintosh II)" }
        };
        private static Dictionary<int, string> _descriptorTypes = new Dictionary<int, string>
        {
            { 0xFF, "Monitor Serial Number" },
            { 0xFE, "Unspecified text" },
            { 0xFD, "Monitor range limits" },
            { 0xFC, "Monitor name" },
            { 0xFB, "Additional white point data" },
            { 0xFA, "Additional standard timing identifiers" }
        };
        #endregion

        #region Properties
        [Browsable(false)]
        public string Name { get { return "EDID"; } }
        [Browsable(false)]
        public static Dictionary<int, string> EstablishedTimings { get { return _establishedTimings; } }
        [Browsable(false)]
        public static Dictionary<int, string> DescriptorTypes { get { return _descriptorTypes; } }
        [Browsable(false)]
        public static string[] WhiteAndSyncLevels { get { return _whiteAndSyncLevels; } }
        [Browsable(false)]
        public static string[] AspectRatios { get{return _aspectRatios; } }
        [Browsable(false)]
        public static string[] DisplayTypesDigital { get { return _displayTypesDigital; } }
        [Browsable(false)]
        public static string[] DisplayTypesAnalog { get { return _displayTypesAnalog; } }
        [Browsable(false)]
        public byte this[int index] { get { return Buffer[index]; } set { Buffer[index] = value; } }
        [Browsable(false)]
        public int Length { get { return Buffer.Length; } }
        [Browsable(false)]
        public byte[] Buffer
        {
            get
            {
                return _buffer;
            }
            set
            {
                _buffer = value;

                Validify();
                OnPropertyChanged("Buffer");
                AllChanged();
            }
        }
        [Category("General")]
        [DisplayName("Manufacturer ID")]
        public string ManufacturerID
        {
            get
            {
                int man = (Buffer[8] << 8) | Buffer[9];
                string toRet = "";
                toRet += (char)(((man >> 10) & 0x1f) + 'A' - 1);
                toRet += (char)(((man >> 5) & 0x1f) + 'A' - 1);
                toRet += (char)((man & 0x1f) + 'A' - 1);
                return toRet;
            }
            set
            {
                string str = "";
                for (int i = 0; i < 3; i++)
                    if (i < value.Length)
                        str += value[i];
                    else
                        str += "A";
                int man = 0;
                man = man | ((str[2] - 'A' + 1) & 0x1f);
                man = man | (((str[1] - 'A' + 1) & 0x1f) << 5);
                man = man | (((str[0] - 'A' + 1) & 0x1f) << 10);
                Buffer[9] = (byte)(man & 0xff);
                Buffer[8] = (byte)((man >> 8) & 0xff);

                Validify();
                OnPropertyChanged("ManufacturerID");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("General")]
        [DisplayName("Product Code")]
        public int ProductCode
        {
            get
            {
                return Buffer[10] | (Buffer[11] << 8);
            }
            set
            {
                Buffer[10] = (byte)(value & 0xff);
                Buffer[11] = (byte)((value >> 8) & 0xff);

                Validify();
                OnPropertyChanged("ProductCode");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("General")]
        [DisplayName("Serial Code")]
        public int SerialCode
        {
            get
            {
                return Buffer[12] | (Buffer[13] << 8) | (Buffer[14] << 16) | (Buffer[15] << 24);
            }
            set
            {
                Buffer[12] = (byte)(value & 0xff);
                Buffer[13] = (byte)((value >> 8) & 0xff);
                Buffer[14] = (byte)((value >> 16) & 0xff);
                Buffer[15] = (byte)((value >> 24) & 0xff);

                Validify();
                OnPropertyChanged("SerialCode");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("Manufactured")]
        [DisplayName("Week")]
        public int WeekOfManufacture
        {
            get
            {
                return Buffer[16];
            }
            set
            {
                Buffer[16] = (byte)(value & 0xff);

                Validify();
                OnPropertyChanged("WeekOfManufacture");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("Manufactured")]
        [DisplayName("Year")]
        public int YearOfManufacture
        {
            get
            {
                return Buffer[17] + 1990;
            }
            set
            {
                Buffer[17] = (byte)((value - 1990) & 0xff);

                Validify();
                OnPropertyChanged("YearOfManufacture");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("EDID")]
        [DisplayName("Version")]
        public int Version
        {
            get
            {
                return Buffer[18];
            }
            set
            {
                Buffer[18] = (byte)(value & 0xff);

                Validify();
                OnPropertyChanged("Version");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("EDID")]
        [DisplayName("Revision")]
        public int Revision
        {
            get
            {
                return Buffer[19];
            }
            set
            {
                Buffer[19] = (byte)(value & 0xff);

                Validify();
                OnPropertyChanged("Revision");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("General")]
        [DisplayName("Digital?")]
        public bool IsDigital
        {
            get
            {
                return GetBit(20, 7);
            }
            set
            {
                SetBit(20, 7, value);

                Validify();
                OnPropertyChanged("IsDigital");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public int RawVideoInputParameters
        {
            get
            {
                return Buffer[20];
            }
            set
            {
                Buffer[20] = (byte)(value & 0xff);

                Validify();
                OnPropertyChanged("VideoInputParameters");
                OnPropertyChanged("Buffer");
            }
        }
        // DIGITAL
        [Category("Digital")]
        [DisplayName("DFP Compatible")]
        public bool IsDFPCompatible
        {
            get
            {
                return GetBit(20, 0);
            }
            set
            {
                SetBit(20, 0, value);

                Validify();
                OnPropertyChanged("IsDFPCompatible");
                OnPropertyChanged("Buffer");
            }
        }
        // ANALOG
        [Category("Analog")]
        [DisplayName("White & Sync")]
        public int WhiteAndSync
        {
            get
            {
                return ((Buffer[20] & 0x60) >> 5);
            }
            set
            {
                byte b = (byte)((value << 5) & 0x60);
                b = (byte)(b | 0x4f);
                Buffer[20] = (byte)(Buffer[20] & b);

                Validify();
                OnPropertyChanged("WhiteAndSync");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("Analog")]
        [DisplayName("Blank To Black")]
        public bool BlankToBlack
        {
            get
            {
                return GetBit(20, 4);
            }
            set
            {
                SetBit(20, 4, value);

                Validify();
                OnPropertyChanged("BlankToBlack");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("Analog")]
        [DisplayName("Seperate Sync")]
        public bool SeperateSync
        {
            get
            {
                return GetBit(20, 3);
            }
            set
            {
                SetBit(20, 3, value);

                Validify();
                OnPropertyChanged("SeperateSync");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("Analog")]
        [DisplayName("Composite Sync")]
        public bool CompositeSync
        {
            get
            {
                return GetBit(20, 2);
            }
            set
            {
                SetBit(20, 2, value);

                Validify();
                OnPropertyChanged("CompositeSync");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("Analog")]
        [DisplayName("Sync On Green")]
        public bool SyncOnGreen
        {
            get
            {
                return GetBit(20, 1);
            }
            set
            {
                SetBit(20, 1, value);

                Validify();
                OnPropertyChanged("SyncOnGreen");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("Analog")]
        [DisplayName("VSync Pulse")]
        public bool VSyncPulse
        {
            get
            {
                return GetBit(20, 0);
            }
            set
            {
                SetBit(20, 0, value);

                Validify();
                OnPropertyChanged("VSyncPulse");
                OnPropertyChanged("Buffer");
            }
        }
        // END
        [Category("General")]
        [DisplayName("Maximal Width")]
        public int MaxHorizontal
        {
            get
            {
                return Buffer[21];
            }
            set
            {
                Buffer[21] = (byte)(value & 0xff);

                Validify();
                OnPropertyChanged("MaxHorizontal");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("General")]
        [DisplayName("Maximal Height")]
        public int MaxVertical
        {
            get
            {
                return Buffer[22];
            }
            set
            {
                Buffer[22] = (byte)(value & 0xff);

                Validify();
                OnPropertyChanged("MaxVertical");
                OnPropertyChanged("Buffer");
            }
        }
        [Category("General")]
        [DisplayName("Gamma")]
        public double Gamma
        {
            get
            {
                return ((double)Buffer[23] + 100.0) / 100.0;
            }
            set
            {
                Buffer[23] = (byte)(((int)((value * 100.0) - 100.0)) & 0xff);

                Validify();
                OnPropertyChanged("Gamma");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public int RawFeatures
        {
            get
            {
                return Buffer[24];
            }
            set
            {
                Buffer[24] = (byte)(value & 0xff);

                Validify();
                OnPropertyChanged("RawFeatures");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public int TimingSupportedRaw
        {
            get
            {
                return Buffer[35] | Buffer[36] << 8 | Buffer[37] << 16;
            }
            set
            {
                Buffer[35] = (byte)(value & 0xff);
                Buffer[36] = (byte)((value >> 8) & 0xff);
                Buffer[37] = (byte)((value >> 16) & 0xff);

                Validify();
                OnPropertyChanged("TimingSupportedRaw");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public int RedX
        {
            get
            {
                return GetColor(27, 25, 6);
            }
            set
            {
                SetColor(27, 25, 6, value);

                Validify();
                OnPropertyChanged("RedX");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public double ClrRedX
        {
            get
            {
                return (double)RedX / 1024.0;
            }
            set
            {
                int v = (int)(value * 1024.0);
                if (v > 1023)
                    v = 1023;
                else if (v < 0)
                    v = 0;
                RedX = v;

                OnPropertyChanged("ClrRedX");
            }
        }
        [Browsable(false)]
        public int RedY
        {
            get
            {
                return GetColor(28, 25, 4);
            }
            set
            {
                SetColor(28, 25, 4, value);

                Validify();
                OnPropertyChanged("RedY");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public double ClrRedY
        {
            get
            {
                return (double)RedY / 1024.0;
            }
            set
            {
                int v = (int)(value * 1024.0);
                if (v > 1023)
                    v = 1023;
                else if (v < 0)
                    v = 0;
                RedY = v;

                OnPropertyChanged("ClrRedY");
            }
        }
        [Browsable(false)]
        public int GreenX
        {
            get
            {
                return GetColor(29, 25, 2);
            }
            set
            {
                SetColor(29, 25, 2, value);

                Validify();
                OnPropertyChanged("GreenX");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public double ClrGreenX
        {
            get
            {
                return (double)GreenX / 1024.0;
            }
            set
            {
                int v = (int)(value * 1024.0);
                if (v > 1023)
                    v = 1023;
                else if (v < 0)
                    v = 0;
                GreenX = v;

                OnPropertyChanged("ClrGreenX");
            }
        }
        [Browsable(false)]
        public int GreenY
        {
            get
            {
                return GetColor(30, 25, 0);
            }
            set
            {
                SetColor(30, 25, 0, value);

                Validify();
                OnPropertyChanged("GreenY");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public double ClrGreenY
        {
            get
            {
                return (double)GreenY / 1024.0;
            }
            set
            {
                int v = (int)(value * 1024.0);
                if (v > 1023)
                    v = 1023;
                else if (v < 0)
                    v = 0;
                GreenY = v;

                OnPropertyChanged("ClrGreenY");
            }
        }
        [Browsable(false)]
        public int BlueX
        {
            get
            {
                return GetColor(31, 26, 6);
            }
            set
            {
                SetColor(31, 26, 6, value);

                Validify();
                OnPropertyChanged("BlueX");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public double ClrBlueX
        {
            get
            {
                return (double)BlueX / 1024.0;
            }
            set
            {
                int v = (int)(value * 1024.0);
                if (v > 1023)
                    v = 1023;
                else if (v < 0)
                    v = 0;
                BlueX = v;

                OnPropertyChanged("ClrBlueX");
            }
        }
        [Browsable(false)]
        public int BlueY
        {
            get
            {
                return GetColor(32, 26, 4);
            }
            set
            {
                SetColor(32, 26, 4, value);

                Validify();
                OnPropertyChanged("BlueY");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public double ClrBlueY
        {
            get
            {
                return (double)BlueY / 1024.0;
            }
            set
            {
                int v = (int)(value * 1024.0);
                if (v > 1023)
                    v = 1023;
                else if (v < 0)
                    v = 0;
                BlueY = v;

                OnPropertyChanged("ClrBlueY");
            }
        }
        [Browsable(false)]
        public int WhiteX
        {
            get
            {
                return GetColor(33, 26, 2);
            }
            set
            {
                SetColor(33, 26, 2, value);

                Validify();
                OnPropertyChanged("WhiteX");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
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
        [Browsable(false)]
        public int WhiteY
        {
            get
            {
                return GetColor(34, 26, 0);
            }
            set
            {
                SetColor(34, 26, 0, value);

                Validify();
                OnPropertyChanged("WhiteY");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public double ClrWhiteY
        {
            get
            {
                return (double)WhiteY / 1024.0;
            }
            set
            {
                int v = (int)(value * 1024.0);
                if (v > 1023)
                    v = 1023;
                else if (v < 0)
                    v = 0;
                WhiteY = v;

                OnPropertyChanged("ClrWhiteY");
            }
        }
        [Browsable(false)]
        public CustomTiming[] CustomTimings { get; private set; }
        [Browsable(false)]
        public bool DPMSStandBy
        {
            get
            {
                return GetBit(24, 7);
            }
            set
            {
                SetBit(24, 7, value);

                Validify();
                OnPropertyChanged("DPMSStandBy");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public bool DPMSSuspend
        {
            get
            {
                return GetBit(24, 6);
            }
            set
            {
                SetBit(24, 6, value);

                Validify();
                OnPropertyChanged("DPMSSuspend");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public bool DPMSActiveOff
        {
            get
            {
                return GetBit(24, 5);
            }
            set
            {
                SetBit(24, 5, value);

                Validify();
                OnPropertyChanged("DPMSActiveOff");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public int DisplayType
        {
            get
            {
                //shift 3
                return (Buffer[24] >> 3) & 0x03;
            }
            set
            {
                Buffer[24] = (byte)(Buffer[24] & (((value & 0x03) << 3) | 0xe7));

                Validify();
                OnPropertyChanged("DisplayType");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public bool StandardsRGB
        {
            get
            {
                return GetBit(24, 2);
            }
            set
            {
                SetBit(24, 2, value);

                Validify();
                OnPropertyChanged("StandardsRGB");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public bool PreferredTiming
        {
            get
            {
                return GetBit(24, 1);
            }
            set
            {
                SetBit(24, 1, value);

                Validify();
                OnPropertyChanged("PreferredTiming");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public bool GTFSupport
        {
            get
            {
                return GetBit(24, 0);
            }
            set
            {
                SetBit(24, 0, value);

                Validify();
                OnPropertyChanged("GTFSupport");
                OnPropertyChanged("Buffer");
            }
        }
        [Browsable(false)]
        public Descriptor[] Descriptors { get; private set; }
        [Browsable(false)]
        public int Extensions
        {
            get
            {
                return Buffer[126];
            }
            set
            {
                Buffer[126] = (byte)(value & 0xff);

                Validify();
                OnPropertyChanged("Extensions");
                OnPropertyChanged("Buffer");
            }
        }
        #endregion

        #region Constructors
        public EDID(byte[] buffer)
        {
            if (buffer == null || buffer.Length != 128)
            {
                Buffer = new byte[128];
                for (int i = 0; i < HEADER.Length; i++)
                    Buffer[i] = HEADER[i];
            }
            else
            {
                Buffer = buffer;
            }
            CustomTimings = new CustomTiming[8];
            for (int i = 0; i < CustomTimings.Length; i++)
                CustomTimings[i] = new CustomTiming(this, 38 + i * 2);
            Descriptors = new Descriptor[4];
            for (int i = 0; i < Descriptors.Length; i++)
                Descriptors[i] = Descriptor.GetDescriptor(this, 54 + i * 18);
        }
        #endregion

        #region Methods
        private bool GetBit(int posByte, int posBit)
        {
            BitArray arr = new BitArray(new byte[] { Buffer[posByte] });
            return arr[posBit];
        }

        private void SetBit(int posByte, int posBit, bool value)
        {
            BitArray arr = new BitArray(new byte[] { Buffer[posByte] });
            arr[posBit] = value;
            Buffer[posByte] = arr.ToByte(true);
        }

        private int GetColor(int byteMost, int byteLeast, int shift)
        {
            return ((Buffer[byteMost] << 2) | ((Buffer[byteLeast] >> shift) & 0x03));
        }

        private void SetColor(int byteMost, int byteLeast, int shift, int value)
        {
            Buffer[byteMost] = (byte)((value >> 2) & 0xff);
            byte restMask = (byte)(((value & 0x03) << shift) | ~(0x03 << shift));
            Buffer[byteLeast] = (byte)(Buffer[byteLeast] & restMask);
        }

        private void AllChanged()
        {
            //OnPropertyChanged("");
        }

        public static EDID FromFile(string path)
        {
            if (IsHex(path))
                return FromHex(path);
            else if (IsTxt(path))
                return FromTxt(path);
            else if (IsDat(path))
                return FromDat(path);

            return null;
        }

        private static bool IsHex(byte[] buffer)
        {
            if (buffer.Length < HEADER.Length)
                return false;

            for (int i = 0; i < HEADER.Length; i++)
                if (buffer[i] != HEADER[i])
                    return false;

            return true;
        }

        private static bool IsHex(string path)
        {
            byte[] buffer = new byte[8];
            using (FileStream fs = File.OpenRead(path))
            {
                fs.Read(buffer, 0, 8);
            }
            return IsHex(buffer);
        }

        private static EDID FromHex(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] buffer = new byte[128];
                fs.Read(buffer, 0, 128);
                return new EDID(buffer);
            }
        }

        private static bool IsTxt(string path)
        {
            byte[] buffer;
            using (FileStream fs = File.OpenRead(path))
            using (StreamReader sr = new StreamReader(fs))
            {
                buffer = sr.ReadToEnd().GetBytesFromHex(8);
            }
            return IsHex(buffer);
        }

        private static EDID FromTxt(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            using (StreamReader sr = new StreamReader(fs))
            {
                return new EDID(sr.ReadToEnd().GetBytesFromHex(128));
            }
        }

        private static bool IsDat(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            using (StreamReader sr = new StreamReader(fs))
            {
                return sr.ReadLine().StartsWith(HEADER_DAT);
            }
        }

        private static EDID FromDat(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            using (StreamReader sr = new StreamReader(fs))
            {
                string txt = "";
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line[5] == '|')
                        txt += line.Substring(7);
                }
                return new EDID(txt.GetBytesFromHex(128));
            }
        }

        public int FindSerial()
        {
            for (int i = 0; i < PATTERNS.Length; i++)
            {
                Tuple<int, int> match = Buffer.FindPattern(new byte[] { 0x00, 0x00, PATTERNS[i], 0x00 });
                if (match != null)
                {
                    _serialType = PATTERNS[i];
                    return match.Item1;
                }
            }
            return -1;
        }

        public string GetSerial()
        {
            int start = _serialStart;
            if (start < 0)
                start = FindSerial();

            if (start < 0)
                return null;

            return Buffer.GetNullTermString(start + 4, 13);
        }

        public void SetSerial(string serial)
        {
            int start = _serialStart;
            if (start < 0)
                start = FindSerial();

            if (start < 0)
                return;

            if (serial.Length > 13)
                serial = serial.Substring(0, 13);

            bool ended = false;
            for (int i = 0; i < serial.Length || i < 13; i++)
                if (i < serial.Length)
                {
                    Buffer[i + start + 4] = (byte)serial[i];
                }
                else if (ended)
                {
                    Buffer[i + start + 4] = 0x20;
                }
                else if (!ended)
                {
                    Buffer[i + start + 4] = 0x0a;
                    ended = true;
                }
        }

        public void SaveToHex(string path)
        {
            Validify();
            if (File.Exists(path))
                File.Delete(path);
            using (FileStream fs = File.OpenWrite(path))
            {
                fs.Write(Buffer, 0, Buffer.Length);
            }
        }

        public void SaveToTxt(string path)
        {
            Validify();
            if (File.Exists(path))
                File.Delete(path);
            using (FileStream fs = File.OpenWrite(path))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(Buffer.GetHexString());
            }
        }

        public void SaveToDat(string path)
        {
            Validify();
            if (File.Exists(path))
                File.Delete(path);
            using (FileStream fs = File.OpenWrite(path))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(HEADER_DAT);
                sw.WriteLine("__________________________________");
                sw.WriteLine();
                sw.WriteLine("128 BYTES OF EDID CODE:");
                sw.WriteLine("         0   1   2   3   4   5   6   7   8   9");
                sw.WriteLine("      ________________________________________");
                for (int i = 0; i < 13; i++)
                {
                    sw.Write("{0,3}  |", i * 10);
                    for (int j = i * 10; j < (i + 1) * 10 && j < Buffer.Length; j++)
                    {
                        sw.Write("  {0:X2}", Buffer[j]);
                    }
                    sw.WriteLine();
                }
            }
        }

        public void Validify()
        {
            int sum = 0;
            for (int i = 0; i < Buffer.Length - 1; i++)
            {
                sum += Buffer[i];
                sum = sum % 256;
            }
            Buffer[127] = (byte)(256 - sum);
        }
        #endregion
    }
}
