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
        WindowState wsl;
        Window window;
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

        void window_StateChanged(object sender, EventArgs e)
        {
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
                window.Hide();
                window.Topmost = false;
            }

            
        }

        void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsCanClose)
            {
                e.Cancel = true;
                window.Hide();
                window.Topmost = false;
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
                window.WindowState = wsl;
                window.Show();
               
            });


            this.notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            AddMenuItem("打开随身袋", delegate
            {
                window.Topmost = true;
                window.WindowState = wsl;
                window.Show();
            });
            this.notifyIcon.ContextMenu.MenuItems.Add("-");
            AddMenuItem("退出",delegate { IsCanClose = true; window.Close(); });
            //this.notifyIcon.ShowBalloonTip(1000);
        }

        public void AddMenuItem(string name,Action action)
        {
            System.Windows.Forms.MenuItem item = new System.Windows.Forms.MenuItem(name);
            item.Click += new EventHandler(delegate { action.Invoke(); });

            this.notifyIcon.ContextMenu.MenuItems.Add(item);
        }
    }
}
