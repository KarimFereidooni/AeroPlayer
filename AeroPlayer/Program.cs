using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AeroPlayer
{
    class Program
    {
        public static bool CanDragMainWindow = true;

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        public static void Main(string[] args)
        {
            bool createdNew = true;
            using (Mutex mutex = new Mutex(true, "AeroPlayer", out createdNew))
            {
                if (createdNew)
                {
                    AeroPlayer.App app = new AeroPlayer.App();
                    app.InitializeComponent();
                    app.Run();
                }
                else
                {
                    Process CurrentProcess = Process.GetCurrentProcess();
                    foreach (Process item in Process.GetProcessesByName(CurrentProcess.ProcessName))
                    {
                        if (item.Id != CurrentProcess.Id)
                        {
                            if (args.Length > 0)
                            {
                                SendMessage(item.MainWindowHandle, args[0]);
                            }
                            Win32.SetForegroundWindow(item.MainWindowHandle);
                            break;
                        }
                    }
                }
            }
        }

        static int SendMessage(IntPtr hWnd, string message)
        {
            int result = 0;
            if (hWnd != IntPtr.Zero)
            {
                string Encoded_Message = Program.EncodeString(message);
                Win32.DataStruct _DataStruct;
                _DataStruct.DW = (IntPtr)100;
                _DataStruct.LP = Encoded_Message;
                _DataStruct.CB = Encoded_Message.Length + 1;
                result = Win32.SendMessage(hWnd, Win32.WM_COPYDATA, IntPtr.Zero, ref _DataStruct);
            }
            return result;
        }

        public static string EncodeString(string text)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            string result = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                result += (bytes[i]).ToString();
                if (i < bytes.Length - 1)
                    result += '|';
            }
            return result;
        }
        public static string DecodeString(string text)
        {
            string[] arr = text.Split('|');
            byte[] bytes = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                bytes[i] = byte.Parse(arr[i]);
            }
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
