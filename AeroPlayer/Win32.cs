using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AeroPlayer
{
    public static class Win32
    {
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref DataStruct lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public const int WM_COPYDATA = 0x4A;

        public struct DataStruct
        {
            public IntPtr DW;
            public int CB;
            [MarshalAs(UnmanagedType.LPStr)]
            public string LP;
        }
    }
}
