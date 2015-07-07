using System;
using System.IO;
using System.Windows;


using System.Xml.Linq;

namespace Ezhu.AutoUpdater
{
    public class Updater
    {
        private static Updater _instance;
        public static Updater Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Updater();
                }
                return _instance;
            }
        }

        public static void CheckUpdateStatus(string url,string xmlFileName,string zipFileName)
        {
            Ezhu.AutoUpdater.Lib.Constants.RemoteUrl = url;
            Ezhu.AutoUpdater.Lib.Constants.XmlFileName = xmlFileName;
            Ezhu.AutoUpdater.Lib.Constants.ZipFileName = zipFileName;
            System.Threading.ThreadPool.QueueUserWorkItem((s) =>
            {
                var client = new System.Net.WebClient();
                client.DownloadDataCompleted += (x, y) =>
                {
                    try
                    {
                        MemoryStream stream = new MemoryStream(y.Result);

                        XDocument xDoc = XDocument.Load(stream);
                        UpdateInfo updateInfo = new UpdateInfo();
                        XElement root = xDoc.Element("UpdateInfo");
                        updateInfo.AppName = root.Element("AppName").Value;
                        updateInfo.AppVersion = root.Element("AppVersion").Value;
                        updateInfo.RequiredMinVersion = root.Element("RequiredMinVersion").Value;
                        updateInfo.Desc = root.Element("Desc").Value;
                        updateInfo.MD5 = Guid.NewGuid().ToString();
                        updateInfo.UrlZip = Ezhu.AutoUpdater.Lib.Constants.RemoteUrl + Ezhu.AutoUpdater.Lib.Constants.ZipFileName;

                        stream.Close();
                        Updater.Instance.StartUpdate(updateInfo);
                    }
                    catch
                    { }
                };
                client.DownloadDataAsync(new Uri(Ezhu.AutoUpdater.Lib.Constants.RemoteUrl + Ezhu.AutoUpdater.Lib.Constants.XmlFileName));

            });

        }

        public void StartUpdate(UpdateInfo updateInfo)
        {
            Version ver =null;
            Version.TryParse(updateInfo.RequiredMinVersion,out ver);
            if (ver != null && Updater.Instance.CurrentVersion < ver)
            {
                //当前版本比需要的版本小，不更新
                return;
            }

            if (Updater.Instance.CurrentVersion >= new Version(updateInfo.AppVersion))
            {
                //当前版本是最新的，不更新
                return;
            }

            //更新程序复制到缓存文件夹
            string appDir = System.IO.Path.Combine(System.Reflection.Assembly.GetEntryAssembly().Location.Substring(0, System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf(System.IO.Path.DirectorySeparatorChar)));
            string updateFileDir = System.IO.Path.Combine("c:\\temp", "Update");
            if (!Directory.Exists(updateFileDir))
            {
                Directory.CreateDirectory(updateFileDir);
            }
            updateFileDir = System.IO.Path.Combine(updateFileDir, updateInfo.MD5.ToString());
            if (!Directory.Exists(updateFileDir))
            {
                Directory.CreateDirectory(updateFileDir);
            }

            string exePath = System.IO.Path.Combine(updateFileDir, "AutoUpdater.exe");
            File.Copy(System.IO.Path.Combine(appDir, "AutoUpdater.exe"), exePath, true);
            var info = new System.Diagnostics.ProcessStartInfo(exePath);
            info.UseShellExecute = true;
            info.WorkingDirectory = exePath.Substring(0, exePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
            updateInfo.Desc = updateInfo.Desc;
            info.Arguments = "update " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(CallExeName)) + " " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(updateFileDir)) + " " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(appDir)) + " " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(updateInfo.AppName)) + " " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(updateInfo.AppVersion.ToString())) + " " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(updateInfo.UrlZip))+" " + (string.IsNullOrEmpty(updateInfo.Desc) ? "" : Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(updateInfo.Desc)));
            System.Diagnostics.Process.Start(info);
        }

        public bool UpdateFinished = false;

        private string _callExeName;
        public string CallExeName
        {
            get
            {
                if (string.IsNullOrEmpty(_callExeName))
                {
                    _callExeName = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1).Replace(".exe", "");
                }
                return _callExeName;
            }
        }

        /// <summary>
        /// 获得当前应用软件的版本
        /// </summary>
        public virtual Version CurrentVersion
        {
            get { return new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location).ProductVersion); }
        }

        /// <summary>
        /// 获得当前应用程序的根目录
        /// </summary>
        public virtual string CurrentApplicationDirectory
        {
            get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); }
        }
    }
}
