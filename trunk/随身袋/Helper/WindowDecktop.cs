using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace 随身袋.Helper
{
    /// <summary>
    /// 窗体固定在桌面上
    /// </summary>
    public class WindowDecktop
    {
        [DllImport("User32.dll ", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        Window window;
        public WindowDecktop(Window wind)
        {
            window = wind;
            Set();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Ret()
        {
            SetParent(new WindowInteropHelper(window).Handle, IntPtr.Zero);
        }

        /// <summary>
        /// 固定在桌面上
        /// </summary>
        public void Set()
        {
            SetParent(new WindowInteropHelper(window).Handle, FindWindowEx(FindWindow("Progman", null), IntPtr.Zero, "shelldll_defview", null));
        }
    }
}
