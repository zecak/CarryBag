using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Speech.Recognition;
using MahApps.Metro.Controls;
using 随身袋.Models;

namespace 随身袋
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        //private SpeechRecognitionEngine SRE = new SpeechRecognitionEngine();
        public MainWindow()
        {
            InitializeComponent();

            Helper.Global.Init();

             //SRE.SetInputToDefaultAudioDevice();         //<=======默认的语音输入设备，你可以设定为去识别一个WAV文件。
             //           GrammarBuilder GB = new GrammarBuilder();
             //           GB.Append("选择");
             //           GB.Append(new Choices(new string[] { "红色", "绿色" }));
             //           Grammar G = new Grammar(GB);
             //           G.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(G_SpeechRecognized);
             //           SRE.LoadGrammar(G);
             //           SRE.RecognizeAsync(RecognizeMode.Multiple); //<=======异步调用识别引擎，允许多次识别（否则程序只响应你的一句话）
        }

        void G_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //btn_test.Background = e.Result.Text;
            switch (e.Result.Text)
            {
                case "选择红色":
                    btn_test.Content = e.Result.Text;
                    break;
                case "选择绿色":
                    btn_test.Content = e.Result.Text;
                    break;
            }
        }

        private void MetroTabControl_TabItemClosingEvent(object sender, BaseMetroTabControl.TabItemClosingEventArgs e)
        {
            if (e.ClosingTabItem.Header.ToString().StartsWith("sizes"))
                e.Cancel = true;
        }

        //private void Expander_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var obj = e.Source as Expander;
        //    obj.IsExpanded = !obj.IsExpanded;
        //}

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

            foreach (var c in Helper.Global.Categorys.FindAll(m=>m.PID==Guid.Empty))
            {
                var tabItem = new TabItem() { Header = c.Name };
                tabItem.Tag = c;
                ControlsHelper.SetHeaderFontSize(tabItem, 18);

                var stackPanel=new StackPanel();

                foreach (var subc in Helper.Global.Categorys.FindAll(m=>m.PID==c.ID))
                {
                    var expander = new Expander() {Header=subc.Name };
                    expander.Tag = subc;

                    var wrapPanel = new WrapPanel();

                    foreach (var link in Helper.Global.Applinks.FindAll(m=>m.PID==subc.ID))
                    {
                        var border = new Border() { Style = (Style)this.FindResource("LinkBorder") };
                        var label = new Label() { Width = 64, Height = 64, Style = (Style)this.FindResource("LinkLabel") };
                        if (string.IsNullOrWhiteSpace(link.ImgSrc)) { link.ImgSrc = "Res/logo.png"; }
                        var image = new Image() { Source = new BitmapImage(new Uri(link.ImgSrc, UriKind.Relative)) };
                        image.Tag = link;
                        image.MouseLeftButtonDown += image_MouseLeftButtonDown;
                        label.Content = image;
                        border.Child = label;
                        wrapPanel.Children.Add(border);
                    }
                    

                    expander.Content = wrapPanel;
                    stackPanel.Children.Add(expander);
                }
               

                tabItem.Content = stackPanel;

                tabMain.Items.Add(tabItem);
            }

            
        }

        void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            var link=img.Tag as AppLink;
            if(link!=null)
            {
                switch (link.AppType)
                {
                    case LinkType.Sys:
                        break;
                    case LinkType.App:
                        break;
                    case LinkType.Web:

                        break;
                    case LinkType.Oth:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
