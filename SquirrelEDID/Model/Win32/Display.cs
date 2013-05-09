using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SquirrelEDID.Model.Win32
{
    public class DisplayInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] EDID { get; set; }
        public object Additional { get; set; }
    }

    public static class Display
    {
        #region Constants
        internal const uint DIGCF_PRESENT = 0x00000002;
        internal const uint EDD_GET_DEVICE_INTERFACE_NAME = 0x1;
        internal const uint DICS_FLAG_GLOBAL = 0x00000001;
        internal const uint DIREG_DEV = 0x00000001;
        internal const uint KEY_READ = 0x20019;
        internal const uint ERROR_NO_MORE_ITEMS = 259; 
        #endregion

        #region Fields
        private static Guid _guidDisplay = new Guid(0x4d36e96e, 0xe325, 0x11ce, 0xbf, 0xc1, 0x08, 0x00, 0x2b, 0xe1, 0x03, 0x18); 
        #endregion

        public static IEnumerable<DisplayInfo> GetDisplays()
        {
            // Get attached DisplayDevices
            List<Win32DisplayDevice> devices = new List<Win32DisplayDevice>();
            for (uint i = 0; ; i++)
            {
                Win32DisplayDevice d = new Win32DisplayDevice();
                uint flags = EDD_GET_DEVICE_INTERFACE_NAME;
                d.cb = Marshal.SizeOf(d);

                bool available = NativeMethods.EnumDisplayDevices(null, i, ref d, flags);
                if (!available)
                    break;
                
                // Zusätzliche Abfrage, um zu schauen, ob eine einzigartige ID für das Geärt vorhanden ist
                Win32DisplayDevice d2 = new Win32DisplayDevice();
                d2.cb = Marshal.SizeOf(d2);
                bool uniqueAvail = NativeMethods.EnumDisplayDevices(d.DeviceName, 0, ref d2, flags);

                if (uniqueAvail)
                    d2.DeviceName = d.DeviceName;

                if (uniqueAvail)
                    devices.Add(d2);
                else
                    devices.Add(d);
            }
            // Get EDIDs from Registry and match with DisplayDevices
            IntPtr devinfo = NativeMethods.SetupDiGetClassDevsEx(
                    ref _guidDisplay, //class GUID
                    0, //enumerator
                    IntPtr.Zero, //HWND
                    DIGCF_PRESENT, // Flags //DIGCF_ALLCLASSES|
                    IntPtr.Zero, // device info, create a new one.
                    null, // machine name, local machine
                    IntPtr.Zero);// reserved

            SP_DEVINFO_DATA devdata = new SP_DEVINFO_DATA();
            devdata.cbSize = (UInt32)Marshal.SizeOf(devdata);

            for (uint i = 0; ; i++)
            {
                NativeMethods.SetupDiEnumDeviceInfo(devinfo, i, out devdata);
                if (Marshal.GetLastWin32Error() == ERROR_NO_MORE_ITEMS)
                    break;

                IntPtr HKEY = NativeMethods.SetupDiOpenDevRegKey(devinfo, ref devdata, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
                if (HKEY == IntPtr.Zero)
                    continue;

                for (uint r = 0; ; r++)
                {
                    uint size = 128;
                    StringBuilder sb = new StringBuilder();
                    uint type = 0;
                    uint len = 1024;
                    IntPtr buf = Marshal.AllocHGlobal((int)len);
                    uint res = NativeMethods.RegEnumValue(HKEY, r, sb, ref size, IntPtr.Zero, ref type, buf, ref len);
                    if (res != ERROR_NO_MORE_ITEMS && sb.ToString(0, (int)size).Equals("EDID"))
                    {
                        byte[] buffer = new byte[len];
                        Marshal.Copy(buf, buffer, 0, (int)len);

                        //match
                        string name = "-";
                        string desc = "unknown";
                        for (int d = 0; d < devices.Count; d++)
                        {
                            Win32DisplayDevice dev = devices[d];
                            int inst = 0;
                            if (!int.TryParse(dev.Key.Substring(dev.Key.LastIndexOf('\\') + 1), out inst))
                                continue;

                            if (inst == devdata.devInst)
                            {
                                name = dev.DeviceName;
                                desc = dev.DeviceString;
                            }
                        }
                        yield return new DisplayInfo { Name = name, Description = desc, EDID = buffer };
                    }
                    Marshal.FreeHGlobal(buf);
                    if (res == ERROR_NO_MORE_ITEMS)
                        break;
                }
            }
        }
    }
}
