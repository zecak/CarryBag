using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            var img= System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        new System.Windows.Int32Rect(0, 0, icon.Width, icon.Height),
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            return img;

        }

        #region 公共操作方法
        public static BitmapSource MakePicture(string bgImagePath, string signature)
        {

            //获取背景图
            BitmapSource bgImage = new BitmapImage(new Uri(bgImagePath, UriKind.Relative));
            //获取头像
            //BitmapSource headerImage = new BitmapImage(new Uri(headerImagePath, UriKind.Relative));

            //创建一个RenderTargetBitmap 对象，将WPF中的Visual对象输出
            RenderTargetBitmap composeImage = new RenderTargetBitmap(bgImage.PixelWidth, bgImage.PixelHeight, bgImage.DpiX, bgImage.DpiY, PixelFormats.Default);

            FormattedText signatureTxt = new FormattedText(signature,
                                                   System.Globalization.CultureInfo.CurrentCulture,
                                                   System.Windows.FlowDirection.LeftToRight,
                                                   new Typeface(System.Windows.SystemFonts.MessageFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                                                   50,
                                                   System.Windows.Media.Brushes.White);



            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(bgImage, new Rect(0, 0, bgImage.Width, bgImage.Height));

            //计算头像的位置
            //double x = (bgImage.Width / 2 - headerImage.Width) / 2;
            //double y = (bgImage.Height/2 - headerImage.Height) / 2 ;
            //drawingContext.DrawImage(headerImage, new Rect(x, y, headerImage.Width, headerImage.Height));

            //计算签名的位置
            double x2 = (bgImage.Width / 2 - signatureTxt.Width) / 2;
            double y2 = (bgImage.Height / 2 - signatureTxt.Height) / 2;
            drawingContext.DrawText(signatureTxt, new System.Windows.Point(x2, y2));
            drawingContext.Close();
            composeImage.Render(drawingVisual);

            ////定义一个JPG编码器
            //JpegBitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
            ////加入第一帧
            //bitmapEncoder.Frames.Add(BitmapFrame.Create(composeImage));

            ////保存至文件（不会修改源文件，将修改后的图片保存至程序目录下）
            //string savePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\合成.jpg";
            //bitmapEncoder.Save(File.OpenWrite(Path.GetFileName(savePath)));
            return composeImage;
        }

        public static BitmapSource Text2Pic(string text)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            // 画矩形
            Rect rect = new Rect(new Point(2, 2), new Size(32, 16));
            drawingContext.DrawRectangle(Brushes.LightBlue, (Pen)null, rect);
            // 画文字
            drawingContext.DrawText(
               new FormattedText(text,
                  System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                  FlowDirection.LeftToRight,
                  new Typeface("Verdana"),
                  36, Brushes.Black),
                  new Point(100, 60));

            drawingContext.Close();

            // 利用RenderTargetBitmap对象，以保存图片
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)32, (int)16, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(drawingVisual);
            return renderBitmap;
        }

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
