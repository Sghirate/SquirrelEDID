using SquirrelEDID.Utilities.Extensions;
using System;
using System.Collections;
using System.Timers;

namespace SquirrelEDID.Model
{
    public enum ProgrammerStatus : byte
    {
        Ready = 0x3e,
        Closed = 0x3f,
        Reading = 0x3a,
        Writing = 0x36,
        Done = 0x32,
        Error = 0xff
    }

    public class Programmer : NotifyPropertyChanged
    {
        #region Fields
        private const byte LED_READY = 0x3e;
        private const byte LED_READ = 0x3a;
        private const byte LED_WRITE = 0x36;
        private const byte LED_DONE = 0x32;
        private const byte LED_CLOSED = 0x3f;
        private const uint DDC_PROGRAMMER_PRODUCT_ID = 5377;
        private static byte[] PINS_READY = new byte[] { 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private static byte[] PINS_READ_EDID = new byte[] { 0x02, 0xc2, 0xa0, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private static byte[] RESPONSE_READ_EDID_START = new byte[] { 0x02, 0x02 };
        private static byte[] PINS_READING_EDID = new byte[] { 0x03, 0x04, 0xa1, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private static byte[] RESPONSE_READING_EDID_START = new byte[] { 0x03, 0x04 };
        private static byte[] RESPONSE_READING_EDID_END = new byte[] { 0x00, 0x00 };
        private static byte[] PINS_WRITE_EDID_START = new byte[] { 0x02, 0xc6, 0xa0 }; // viertes Byte = Position!
        private static byte[] RESPONSE_WRITING_EDID_START = new byte[] { 0x02 };
        private static byte[] PINS_BYE = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private IntPtr _iow;
        private IntPtr _device;
        private System.Timers.Timer _timer;
        private int _doneStep = 0;
        private static bool _inTick = false;
        #endregion

        #region Properties
        public bool WarriorAvailable { get; private set; }
        public bool Attached { get; private set; }
        public ProgrammerStatus Status { get; private set; }
        #endregion

        public event EventHandler OnAttach;

        #region Constructors
        public Programmer()
        {
            Status = ProgrammerStatus.Closed;
            Attached = false;
            WarriorAvailable = GetWarrior();
            _timer = new System.Timers.Timer(100);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }
        #endregion

        #region Destructors
        ~Programmer()
        {
            _timer.Stop();
            if (Attached)
            {
                SetPins(PINS_BYE);
                SetLED(LED_CLOSED);
            }
            if(WarriorAvailable)
                Functions.IowKitCloseDevice(_iow);
        }
        #endregion

        #region Methods
        private bool GetWarrior()
        {
            try
            {
                _iow = Functions.IowKitOpenDevice();
                WarriorAvailable = true;
                return true;
            }
            catch (Exception ex)
            {
                WarriorAvailable = false;
                return false;
            }
        }

        private bool GetDevice()
        {
            if (!WarriorAvailable && !GetWarrior())
                return false;

            try
            {
                bool oldAttached = Attached;
                IntPtr device = IntPtr.Zero;
                uint numDevs = Functions.IowKitGetNumDevs();
                if (numDevs < 1)
                    return false;

                for (uint i = 1; i <= numDevs; i++)
                {
                    IntPtr handle = Functions.IowKitGetDeviceHandle(i);
                    uint productID = Functions.IowKitGetProductId(handle);
                    if (productID == DDC_PROGRAMMER_PRODUCT_ID)
                    {
                        device = handle;
                        Attached = true;
                    }
                }

                if (Attached)
                {
                    if (!oldAttached || (_device != IntPtr.Zero && _device != device))
                    {
                        _device = device;
                        OnAttached();
                    }
                }
                else
                {
                    _device = IntPtr.Zero;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void OnAttached()
        {
            if (!SetLED(LED_READY))
            {

            }
            //byte[] msg = GetLEDMessage(LED_READY);
            //Functions.IowKitWrite(_device, 0, msg, Len(msg));
            if (SetLED(LED_READY))
                Status = ProgrammerStatus.Ready;
            Functions.IowKitWrite(_device, 1, PINS_READY, Len(PINS_READY));
            Functions.IowKitSetTimeout(_device, 100);
            //Status = ProgrammerStatus.Ready;
        }

        private void Detach()
        {

        }

        uint Len(byte[] arr)
        {
            return (uint)arr.Length;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_inTick)
                return;
            _inTick = true;

            if (!Attached && !GetDevice())
                return;

            switch (Status)
            {
                case ProgrammerStatus.Closed:
                    byte led = GetLED();
                    if (led > 0 && led == LED_READY)
                        Status = ProgrammerStatus.Ready;
                    else
                        SetLED(LED_READY);
                    break;
                case ProgrammerStatus.Ready:
                    // chill - for now
                    break;
                case ProgrammerStatus.Done:
                    if (_doneStep < 1)
                    {
                        if (SetLED(LED_READY))
                            _doneStep += 1;
                    }
                    else if (_doneStep < 2)
                    {
                        if (SetLED(LED_DONE))
                            _doneStep += 1;
                    }
                    else if (_doneStep < 3)
                    {
                        _doneStep += 1;
                    }
                    else if (_doneStep < 4)
                    {
                        if (SetLED(LED_READY))
                            Status = ProgrammerStatus.Ready;
                    }
                    else
                    {
                        _doneStep = 0;
                    }
                    break;
                case ProgrammerStatus.Error:
                    // NADA
                    break;
            }

            _inTick = false;
        }

        private byte GetLEDCommand(bool status, bool read, bool write)
        {
            return new BitArray(new bool[] { !status, true, !read, !write, true, true }).ToByte(true);
        }

        private bool SetLED(bool status, bool read, bool write)
        {
            byte b = GetLEDCommand(status, read, write);
            return SetLED(b);
        }

        private bool GetSetLED(bool status, bool read, bool write)
        {
            byte b = GetLEDCommand(status, read, write);
            return SetGetLED(b);
        }

        private bool SetGetLED(byte b)
        {
            if (!SetLED(b))
                return false;
            return GetLED() == b;
        }

        private bool SetLED(byte b)
        {
            if (!Attached && !GetDevice())
                return false;

            try
            {
                uint bytesRead = 0;
                byte[] buffer = new byte[] { 0x00, 0x00, b };
                Functions.IowKitWrite(_device, 0, buffer, Len(buffer));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private byte GetLED()
        {
            if (!Attached && !GetDevice())
                return 0;

            try
            {
                uint bytesRead = 0;
                byte[] buffer = new byte[3];
                bytesRead = Functions.IowKitRead(_device, 0, buffer, (uint)buffer.Length);
                if (bytesRead > 0)
                    return buffer[2];
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private byte[] SetPins(byte[] msg)
        {
            if (!Attached && !GetDevice())
                return null;

            uint bytesRead = 0;
            byte[] buffer = new byte[msg.Length];
            Functions.IowKitWrite(_device, 1, msg, Len(msg));
            bytesRead = Functions.IowKitRead(_device, 1, buffer, (uint)buffer.Length);
            if (bytesRead < 1)
                return null;
            return buffer;
        }

        private bool SetPins4EDIDBytes(byte[] edid, int start)
        {
            if (!Attached && !GetDevice())
                return false;

            byte[] msg = new byte[8];
            for (int i = 0; i < PINS_WRITE_EDID_START.Length; i++)
                msg[i] = PINS_WRITE_EDID_START[i];
            msg[3] = (byte)start;
            for (int i = 0; i < 4; i++)
                msg[i + 4] = edid[start + i];

            byte[] buffer = SetPins(msg);
            if (buffer == null)
                return false;
            return buffer.StartsWithPattern(RESPONSE_WRITING_EDID_START);
        }

        private byte[] PrivateRead()
        {
            if (!SetLED(LED_READ))
                return null;

            byte[] buffer = SetPins(PINS_READ_EDID);
            if (buffer == null)
                return null;
            else if (!buffer.StartsWithPattern(RESPONSE_READ_EDID_START))
                return null;

            byte[] edid = new byte[128];
            for (int i = 0; i < 32; i++)
            {
                buffer = SetPins(PINS_READING_EDID);
                if (buffer == null)
                    return null;
                else if (!buffer.StartsWithPattern(RESPONSE_READING_EDID_START) || !buffer.EndsWithPattern(RESPONSE_READING_EDID_END))
                    return null;

                for (int b = 0; b < 4; b++)
                    edid[i * 4 + b] = buffer[b + 2];
            }
            return edid;
        }

        private bool PrivateWrite(byte[] edid)
        {
            if (edid.Length < 128)
                return false;

            if (!SetLED(LED_WRITE))
                return false;

            for (int i = 0; i < 32; i++)
                if (!SetPins4EDIDBytes(edid, i * 4))
                    return false;

            return true;
        }

        public byte[] ReadEDID()
        {
            byte[] edid = PrivateRead();
            /*if (edid == null)
                Status = ProgrammerStatus.Error;*/
            Done();
            return edid;
        }

        public bool WriteEDID(byte[] data)
        {
            if (data.Length < 128)
                return false;

            PrivateWrite(data);

            byte[] written = PrivateRead();
            if (written == null)
                return false;

            if (written.Length < 128)
                return false;

            for (int i = 0; i < 128; i++)
                if (written[i] != data[i])
                    return false;
            Done();
            return true;
        }

        private void Done()
        {
            _doneStep = 0;
            Status = ProgrammerStatus.Done;
        }
        #endregion
    }
}
