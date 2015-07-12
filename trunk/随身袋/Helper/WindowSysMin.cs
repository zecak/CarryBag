using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace 随身袋.Helper
{
    public class WindowSysMin
    {

        System.Windows.Forms.NotifyIcon notifyIcon;
        WindowState ws;
        WindowState wsl;
        Window window;
        System.Windows.Controls.ContextMenu contextMenu;
        bool IsCanClose = false;
        public WindowSysMin(Window wind)
        {
            window = wind;
            window.Closing += window_Closing;
            window.StateChanged += window_StateChanged;
            //保证窗体显示在上方。
            wsl = window.WindowState;

            Init();
        }
        public WindowSysMin(Window wind,System.Windows.Controls.ContextMenu menu)
        {
            window = wind;
            window.Closing += window_Closing;
            window.StateChanged += window_StateChanged;

            contextMenu = menu;

            //保证窗体显示在上方。
            wsl = window.WindowState;

            Init();
        }

        void window_StateChanged(object sender, EventArgs e)
        {
            ws = window.WindowState;
            if (ws == WindowState.Minimized)
            {
                window.Hide();
            }
        }

        void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsCanClose)
            {
                e.Cancel = true;
                window.Hide();
            }
        }
        void Init()
        {
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            //this.notifyIcon.BalloonTipText = "你好, 欢迎使用随身袋!";
            this.notifyIcon.Text = window.Title;
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.FriendlyName);
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new EventHandler(delegate
            {
                window.Topmost = true;
                window.Show();
                window.WindowState = wsl;
            });

            this.notifyIcon.MouseClick += notifyIcon_MouseClick;

            //this.notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();

            //System.Windows.Forms.MenuItem closeItem = new System.Windows.Forms.MenuItem("退出");
            //closeItem.Click += new EventHandler(delegate { IsCanClose = true; window.Close(); });

            //this.notifyIcon.ContextMenu.MenuItems.Add(closeItem);
            //this.notifyIcon.ShowBalloonTip(1000);
        }
        public delegate void OnContextMenu();
        public event OnContextMenu RightContextMenuShow;
        void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (RightContextMenuShow != null)
                {
                    RightContextMenuShow();
                }
                if (contextMenu!=null)
                {
                    contextMenu.IsOpen = true;
                }
            }

        }
    }
}
