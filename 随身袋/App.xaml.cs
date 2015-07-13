using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace 随身袋
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static System.Threading.Mutex run;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
                bool runone;
                run = new System.Threading.Mutex(true, "App_" + "CarryBag", out runone);
                if (!runone)
                {
                    Application.Current.Shutdown();
                    return;
                }

                //FSLib.App.SimpleUpdater.Updater.CheckUpdateSimple(curClient.更新Url, "update_c.xml");
                try
                {
                    Ezhu.AutoUpdater.Updater.CheckUpdateStatus("http://update.zecak.com/CarryBag/", "update.xml", "update.zip");
                }
                catch { }

                Application currApp = Application.Current;
                currApp.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);

                run.ReleaseMutex();

            
        }

    }
}
