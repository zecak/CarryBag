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
        public static List<RootCategory> Categorys =null;

        public const string ApplinkFileName = "applink.xml";
        public static List<AppLink> Applinks = null;
        public static void Init() 
        {
            Categorys = XMLDes(SettingFileName) as List<RootCategory>;
            if(Categorys==null||Categorys.Count==0)
            {
                Categorys = new List<RootCategory>() { 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="软件", PID=Guid.Empty, SortNum=1}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="网址", PID=Guid.Empty, SortNum=2}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="系统", PID=Guid.Empty, SortNum=3}, 
                    new RootCategory(){ ID=Guid.NewGuid(), Name="其他", PID=Guid.Empty, SortNum=4}, 
                };
                XMLSer(SettingFileName, Categorys);
            }

            Applinks = XMLDes(ApplinkFileName) as List<AppLink>;
            if (Applinks == null || Applinks.Count == 0)
            {
                var category = Categorys.FirstOrDefault(m => m.Name == "网址");
                var subcategory = new RootCategory() { ID = Guid.NewGuid(), Name = "官方网站", PID = category.ID, SortNum = 1 };
                Categorys.Add(subcategory);
                XMLSer(SettingFileName, Categorys);
                Applinks = new List<AppLink>() { 
                    new AppLink(){ ID=Guid.NewGuid(),AppType= LinkType.Web, Name="泽卡可", PID=subcategory.ID, SortNum=1, FileName="http://www.zecak.com"  }, 

                };
                XMLSer(ApplinkFileName, Applinks);
            }
        }

        #region 公共操作方法


        public static object XMLDes(string filename)
        {
            try
            {
                return EasyFramework.Serialize.XMLHelper.Deserialize<object>(filename);
            }
            catch
            {
                return null;
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
