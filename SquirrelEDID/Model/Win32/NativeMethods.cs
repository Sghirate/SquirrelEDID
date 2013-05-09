using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SquirrelEDID.Model.Win32
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GlobalFree(HandleRef handle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref Win32DisplayDevice lpDisplayDevice, uint dwFlags);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPlacement(IntPtr hWnd, [In]ref Win32WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, out Win32WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll")]
        internal static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode)] 
        public static extern IntPtr SetupDiGetClassDevsEx(ref Guid GuidClass, UInt32 Enumerator, IntPtr hParent, UInt32 nFlags, IntPtr DeviceInfoSet, [MarshalAs(UnmanagedType.LPWStr)] string MachineName, IntPtr Reserved);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, UInt32 memberIndex, [Out] out SP_DEVINFO_DATA deviceInfoData);

        [DllImport("Setupapi", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiOpenDevRegKey(IntPtr hDeviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint scope, uint hwProfile, uint parameterRegistryValueKind, uint samDesired);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern uint RegEnumValue(IntPtr hKey, uint dwIndex, StringBuilder lpValueName, ref uint lpcValueName, IntPtr lpReserved, ref uint lpType, IntPtr data, ref uint len);
    }
}
