using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 随身袋.Models;

namespace 随身袋.Helper
{
    public class Global
    {
        public const string SettingFileName = "setting.xml";
        private static List<RootCategory> categorys = null;

        public const string ApplinkFileName = "applink.xml";
        private static List<AppLink> applinks = null;
        public static void Init() 
        {
            categorys = XMLDes<List<RootCategory>>(SettingFileName);
            if (categorys == null || categorys.Count == 0)
            {
                categorys = new List<RootCategory>() { 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="软件", PID=Guid.Empty, SortNum=1}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="网址", PID=Guid.Empty, SortNum=2}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="系统", PID=Guid.Empty, SortNum=3}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="其他", PID=Guid.Empty, SortNum=4}, 
                };
                XMLSer(SettingFileName, categorys);
            }

            applinks = XMLDes<List<AppLink>>(ApplinkFileName);
            if (applinks == null || applinks.Count == 0)
            {
                var category = Categorys.FirstOrDefault(m => m.Name == "网址");
                var subcategory = new RootCategory() { ID = Guid.NewGuid(), Name = "官方网站", PID = category.ID, SortNum = 1 };
                Categorys.Add(subcategory);
                XMLSer(SettingFileName, Categorys);
                applinks = new List<AppLink>() { 
                    new AppLink(){ ID=Guid.NewGuid(),AppType= LinkType.Web, Name="泽卡可", PID=subcategory.ID, SortNum=1, FileName="http://www.zecak.com"  }, 

                };
                XMLSer(ApplinkFileName, applinks);
            }
        }

        public static List<RootCategory> Categorys
        {
            get
            {
                if (categorys != null) { return categorys; }
                categorys = XMLDes<List<RootCategory>>(SettingFileName);
                if (categorys == null || categorys.Count == 0)
                {
                    categorys = new List<RootCategory>() { 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="软件", PID=Guid.Empty, SortNum=1}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="网址", PID=Guid.Empty, SortNum=2}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="系统", PID=Guid.Empty, SortNum=3}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="其他", PID=Guid.Empty, SortNum=4}, 
                    };
                    XMLSer(SettingFileName, categorys);
                }
                return categorys;
            }
 
        }

        public static void SaveCategorys()
        {
            XMLSer(SettingFileName, categorys);
        }

        public static void SaveAppLinks()
        {
            XMLSer(ApplinkFileName, applinks);
        }

        public static List<AppLink> AppLinks 
        {
            get
            {
                if (applinks != null) { return applinks; }
                applinks = XMLDes<List<AppLink>>(ApplinkFileName);
                if (applinks == null || applinks.Count == 0)
                {
                    var category = categorys.FirstOrDefault(m => m.Name == "网址");
                    var subcategory = new RootCategory() { ID = Guid.NewGuid(), Name = "官方网站", PID = category.ID, SortNum = 1 };
                    categorys.Add(subcategory);
                    XMLSer(SettingFileName, categorys);
                    applinks = new List<AppLink>() { 
                    new AppLink(){ ID=Guid.NewGuid(),AppType= LinkType.Web, Name="泽卡可", PID=subcategory.ID, SortNum=1, FileName="http://www.zecak.com"  }, 

                    };
                    XMLSer(ApplinkFileName, applinks);
                }
                return applinks;
            }

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
