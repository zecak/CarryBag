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
        public static List<RootCategory> Categorys = new List<RootCategory>();

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
