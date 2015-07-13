using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace 随身袋.Helper
{
    public class WindowAutoHide
    { 
        //窗体自动隐藏
        private bool _IsHidded = false;
        private const int BORDER = 3;
        private const int AUTOHIDETIME = 50;
        private Location location = Location.None;
        private DispatcherTimer autoHideTimer = null;
        
        enum Location
        {
            None,
            Top,
            LeftTop,
            RightTop
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);
        Window window;
        public WindowAutoHide(Window wind)
        {
            window = wind;
            window.LocationChanged += window_LocationChanged;
            //自动隐藏窗体
            this.autoHideTimer = new DispatcherTimer();
            this.autoHideTimer.Interval = TimeSpan.FromMilliseconds(AUTOHIDETIME);
            this.autoHideTimer.Tick += new EventHandler(AutoHideTimer_Tick);
        }

        void window_LocationChanged(object sender, EventArgs e)
        {
            if (!this._IsHidded)
            {
                if (window.Top <= 0 && window.Left <= 0)
                {
                    this.location = Location.LeftTop;
                    this.HideWindow();
                }
                else if (window.Top <= 0 && window.Left >= SystemParameters.VirtualScreenWidth - window.ActualWidth)
                {
                    this.location = Location.RightTop;
                    this.HideWindow();
                }
                else if (window.Top <= 0)
                {
                    this.location = Location.Top;
                    this.HideWindow();
                }
                else
                {
                    this.location = Location.None;
                }
            }
        }

        /// <summary>
        /// 创建一个计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoHideTimer_Tick(object sender, EventArgs e)
        {

            POINT p;
            if (!GetCursorPos(out p))
            {
                return;
            }

            if (p.x >= window.Left && p.x <= (window.Left + window.ActualWidth)
                 && p.y >= window.Top && p.y <= (window.Top + window.ActualHeight))
            {
                this.ShowWindow();
            }
            else
            {
                this.HideWindow();
            }
        }

        /// <summary>
        /// 隐藏窗体
        /// </summary>
        private void ShowWindow()
        {
            if (this._IsHidded)
            {
                switch (this.location)
                {
                    case Location.Top:
                    case Location.LeftTop:
                    case Location.RightTop:
                        window.Top = 0;
                        window.Topmost = false;
                        this._IsHidded = false;
                        window.UpdateLayout();
                        break;
                    case Location.None:
                        break;
                }
            }
        }
        /// <summary>
        /// 显示窗体
        /// </summary>
        private void HideWindow()
        {
            if (!this._IsHidded)
            {
                switch (this.location)
                {
                    case Location.Top:
                        window.Top = BORDER - window.ActualHeight;
                        window.Topmost = true;
                        this._IsHidded = true;
                        this.autoHideTimer.Start();
                        break;
                    case Location.LeftTop:
                        window.Left = 0;
                        window.Top = BORDER - window.ActualHeight;
                        window.Topmost = true;
                        this._IsHidded = true;
                        this.autoHideTimer.Start();
                        break;
                    case Location.RightTop:
                        window.Left = SystemParameters.VirtualScreenWidth - window.ActualWidth;
                        window.Top = BORDER - window.ActualHeight;
                        window.Topmost = true;
                        this._IsHidded = true;
                        this.autoHideTimer.Start();
                        break;
                    case Location.None:
                        break;
                }
            }
        }
    }
}
