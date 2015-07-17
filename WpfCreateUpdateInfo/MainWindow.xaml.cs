using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfCreateUpdateInfo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var files = System.IO.Directory.GetFiles(txtDir.Text, "随身袋.exe");
                if (files.Length == 1)
                {
                    System.Diagnostics.FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.IO.Path.GetFullPath(files[0]));
                    var asmTitle = fv.ProductName;
                    var asmVer = fv.ProductVersion;
                    txtAppName.Text = asmTitle;
                    txtCVer.Text = asmVer;
                    System.IO.FileInfo file = new System.IO.FileInfo(System.IO.Path.GetFullPath(files[0]));
                    txtUpdateTime.Text = file.LastWriteTime.ToShortDateString();
                }

                txtDes.Text = System.IO.File.ReadAllText((@"..\..\..\随身袋\bin\Release\CarryBag\使用说明.txt"));

                System.IO.File.Delete("update.zip");

                ICCEmbedded.SharpZipLib.Zip.FastZipEvents evt = new ICCEmbedded.SharpZipLib.Zip.FastZipEvents();
                ICCEmbedded.SharpZipLib.Zip.FastZip fz = new ICCEmbedded.SharpZipLib.Zip.FastZip(evt);
                fz.CreateZip("update.zip", txtDir.Text, true, "");

                System.IO.FileInfo file2 = new System.IO.FileInfo(System.IO.Path.GetFullPath("update.zip"));
                txtPackageSize.Text = file2.Length.ToString();

                UpdateInfo uinfo = new UpdateInfo();
                uinfo.AppName = txtAppName.Text;
                uinfo.AppVersion = txtCVer.Text;
                uinfo.Desc = txtDes.Text;
                uinfo.RequiredMinVersion = txtMinVer.Text;
                uinfo.PackageName = txtPackageName.Text;
                uinfo.PackageSize = txtPackageSize.Text;
                uinfo.UpdateTime = txtUpdateTime.Text;

                EasyFramework.Serialize.XMLHelper.Serialize("update.xml", uinfo);
            }
            catch { }
        }
    }
}
