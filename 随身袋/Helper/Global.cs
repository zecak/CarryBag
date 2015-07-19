using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using 随身袋.Models;

namespace 随身袋.Helper
{
    public class Global
    {
        public const string AppBagName = "AppBag";
        static string appPath = null;
        public static string AppPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(appPath)) { appPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase; }
                return appPath;
            }
        }
        static string appBagPath = null;
        public static string AppBagPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(appBagPath)) { appBagPath = System.IO.Path.Combine(AppPath, AppBagName); }
                return appBagPath;
            }
        }
        public const string SettingFileName = "setting.xml";
        private static List<RootCategory> categorys = null;

        public const string ApplinkFileName = "applink.xml";
        private static List<AppLink> applinks = null;
        public static void Init()
        {
            if (!System.IO.Directory.Exists(AppBagPath))
            {
                System.IO.Directory.CreateDirectory(AppBagPath);
            }

            categorys = XMLDes<List<RootCategory>>(System.IO.Path.Combine(AppPath, SettingFileName));
            if (categorys == null || categorys.Count == 0)
            {
                categorys = GetInitRootCategory();
                SaveCategorys();
            }

            applinks = XMLDes<List<AppLink>>(System.IO.Path.Combine(AppPath, ApplinkFileName));
            if (applinks == null)
            {
                var category = Categorys.FirstOrDefault(m => m.Name == "系统");
                var subcategory = new RootCategory() { ID = Guid.NewGuid(), Name = "常用工具", PID = category.ID, SortNum = 1 };
                categorys.Add(subcategory);
                SaveCategorys();

                applinks = GetInitAppLinks(subcategory.ID);
                SaveAppLinks();
            }
        }

        public static string EncodeCtrlName(string name)
        {
            return "EnCN_"+EasyFramework.Security.Encrypt.EncryptHelper.APIEncode(name, EasyFramework.Security.Encrypt.EncryptHelper.EncryptType.MD5);
        }


        public static List<RootCategory> Categorys
        {
            get
            {
                if (categorys != null) { return categorys; }
                categorys = XMLDes<List<RootCategory>>(System.IO.Path.Combine(AppPath, SettingFileName));
                if (categorys == null || categorys.Count == 0)
                {
                    categorys = GetInitRootCategory();
                    SaveCategorys();
                }
                return categorys;
            }

        }

        public static void SaveCategorys()
        {
            XMLSer(System.IO.Path.Combine(AppPath, SettingFileName), categorys);
        }

        public static void SaveAppLinks()
        {
            XMLSer(System.IO.Path.Combine(AppPath, ApplinkFileName), applinks);
        }

        public static List<AppLink> AppLinks
        {
            get
            {
                if (applinks != null) { return applinks; }
                applinks = XMLDes<List<AppLink>>(System.IO.Path.Combine(AppPath, ApplinkFileName));
                if (applinks == null)
                {
                    var category = categorys.FirstOrDefault(m => m.Name == "系统");
                    var subcategory = new RootCategory() { ID = Guid.NewGuid(), Name = "常用工具", PID = category.ID, SortNum = 1 };
                    categorys.Add(subcategory);
                    SaveCategorys();

                    applinks = GetInitAppLinks(subcategory.ID);
                    SaveAppLinks();
                }
                return applinks;
            }

        }

        static List<RootCategory> GetInitRootCategory()
        {
            return new List<RootCategory>() { 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="软件", PID=Guid.Empty, SortNum=1}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="系统", PID=Guid.Empty, SortNum=2}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="娱乐", PID=Guid.Empty, SortNum=3}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="目录", PID=Guid.Empty, SortNum=4}, 
                    };
        }

        static List<AppLink> GetInitAppLinks(Guid gid)
        {
            return new List<AppLink>() { 
                    new AppLink(){ ID=Guid.NewGuid(), Name="文本编辑", PID=gid, SortNum=1, FileName=@"C:\Windows\System32\notepad.exe" , IsRelative=false, Tags="系统,文本编辑器,notepad" }, 
                    new AppLink(){ ID=Guid.NewGuid(), Name="计算器", PID=gid, SortNum=1, FileName=@"C:\Windows\System32\calc.exe" , IsRelative=false,Tags="系统,计算器,calc" }, 
                     new AppLink(){ ID=Guid.NewGuid(), Name="命令提示符", PID=gid, SortNum=1, FileName=@"C:\Windows\System32\cmd.exe" , IsRelative=false,Tags="系统,命令提示符,cmd" }, 

                    };
        }

        public static ImageSource GetIcon(string fileName)
        {
            System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        new System.Windows.Int32Rect(0, 0, icon.Width, icon.Height),
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }

        #region 公共操作方法


        public static T XMLDes<T>(string filename)
        {
            try
            {
                return EasyFramework.Serialize.XMLHelper.Deserialize<T>(filename);
            }
            catch
            {
                return default(T);
            }

        }

        public static void XMLSer(string filename, object obj)
        {
            try
            {
                EasyFramework.Serialize.XMLHelper.Serialize(filename, obj);
            }
            catch
            {
            }
        }


        #endregion
    }
}
