using System;
using System.Runtime.InteropServices;

namespace SquirrelEDID.Model.Win32
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32WindowPlacement
    {
        #region Fields
        public int length;
        public int flags;
        public int showCmd;
        public Win32Point minPosition;
        public Win32Point maxPosition;
        public Win32Rect normalPosition;
        #endregion
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Rect
    {
        #region Fields
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        #endregion

        #region Constructors
        public Win32Rect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (!(obj is Win32Rect))
                return false;

            if (ReferenceEquals(obj, this))
                return true;

            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() + Top.GetHashCode() + Right.GetHashCode() + Bottom.GetHashCode();
        }
        #endregion

        #region Operators
        public static bool operator ==(Win32Rect a, Win32Rect b)
        {
            if (a == null || b == null)
                return false;

            if (a.Equals(b))
                return true;

            return a.Left == b.Left && a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Bottom;
        }

        public static bool operator !=(Win32Rect a, Win32Rect b)
        {
            return !(a == b);
        }
        #endregion
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        #region Fields
        public int X;
        public int Y;
        #endregion

        #region Constructors
        public Win32Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (!(obj is Win32Point))
                return false;

            if (ReferenceEquals(obj, this))
                return true;

            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }
        #endregion

        #region Operators
        public static bool operator ==(Win32Point a, Win32Point b)
        {
            if (a == null || b == null)
                return false;

            if (a.Equals(b))
                return true;

            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Win32Point a, Win32Point b)
        {
            return !(a == b);
        }

        public static Win32Point operator +(Win32Point a, Win32Point b)
        {
            return new Win32Point(a.X + b.X, a.Y + b.Y);
        }

        public static Win32Point operator -(Win32Point a, Win32Point b)
        {
            return new Win32Point(a.X - b.X, a.Y - b.Y);
        }

        public static Win32Point operator *(Win32Point a, Win32Point b)
        {
            return new Win32Point(a.X * b.X, a.Y * b.Y);
        }

        public static Win32Point operator /(Win32Point a, Win32Point b)
        {
            return new Win32Point(a.X / b.X, a.Y / b.Y);
        }

        public static Win32Point operator ++(Win32Point a)
        {
            return new Win32Point(a.X++, a.Y++);
        }

        public static Win32Point operator --(Win32Point a)
        {
            return new Win32Point(a.X--, a.Y--);
        }
        #endregion
    }

    [Flags()]
    public enum DisplayDeviceStateFlags : int
    {
        /// <summary>The device is part of the desktop.</summary>
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        /// <summary>The device is part of the desktop.</summary>
        PrimaryDevice = 0x4,
        /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
        MirroringDriver = 0x8,
        /// <summary>The device is VGA compatible.</summary>
        VGACompatible = 0x16,
        /// <summary>The device is removable; it cannot be the primary display.</summary>
        Removable = 0x20,
        /// <summary>The device has more display modes than its output devices support.</summary>
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct Win32DisplayDevice
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        [MarshalAs(UnmanagedType.U4)]
        public uint StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;

        public string Name
        {
            get
            {
                return DeviceName;
            }
        }
        public string ID
        {
            get
            {
                return DeviceID;
            }
        }
        public string String
        {
            get
            {
                return DeviceString;
            }
        }
        public string Key
        {
            get
            {
                return DeviceKey;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SP_DEVINFO_DATA
    {
        public UInt32 cbSize;
        public Guid classGuid;
        public UInt32 devInst;
        public IntPtr reserved;     // CHANGE #1 - was UInt32
    }
}
