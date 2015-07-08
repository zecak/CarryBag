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
using System.Collections.ObjectModel;

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

        ContextMenu SubCMenu = new ContextMenu();

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //SubCMenu.Items.Add(new MenuItem() { Header="添加文件"});
            var mitem_upd = new MenuItem() { Header = "修改" };
            mitem_upd.Click += mitem_upd_Click;
            var mitem_del = new MenuItem() { Header = "删除" };
            mitem_del.Click += mitem_del_Click;
            SubCMenu.Items.Add(mitem_upd);
            SubCMenu.Items.Add(mitem_del);

            cbx_root.ItemsSource = Helper.Global.Categorys.FindAll(m => m.PID == Guid.Empty);
            foreach (var c in Helper.Global.Categorys.FindAll(m => m.PID == Guid.Empty))
            {
                var tabItem = new TabItem() { Header = c.Name };
                tabItem.Tag = c;
                ControlsHelper.SetHeaderFontSize(tabItem, 18);


                var stackPanel = new StackPanel() { Name = c.Name };

                foreach (var subc in Helper.Global.Categorys.FindAll(m => m.PID == c.ID))
                {
                    var expander = new Expander() { Name = subc.Name, Header = subc.Name, IsExpanded = true };
                    expander.Tag = subc;

                    expander.ContextMenu = SubCMenu;

                    var wrapPanel = new WrapPanel();

                    foreach (var link in Helper.Global.AppLinks.FindAll(m => m.PID == subc.ID))
                    {
                        var border = new Border() { Name = link.Name, Style = (Style)this.FindResource("LinkBorder") };
                        var label = new Label() { Width = 64, Height = 64, Style = (Style)this.FindResource("LinkLabel") };
                        if (string.IsNullOrWhiteSpace(link.ImgSrc)) { link.ImgSrc = "Res/logo.png"; }
                        var image = new Image() { Source = new BitmapImage(new Uri(link.ImgSrc, UriKind.Relative)), ToolTip = link.Name };
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

        void mitem_del_Click(object sender, RoutedEventArgs e)
        {
            
            var mbr = MessageBox.Show("确定删除该分类信息么?", "提示", MessageBoxButton.OKCancel);
            if (mbr == MessageBoxResult.OK)
            {
                var exp = SubCMenu.PlacementTarget as Expander;
                var c = exp.Tag as RootCategory;
                var p_c = Helper.Global.Categorys.FirstOrDefault(m => m.ID == c.PID);
                var spanel = this.tabMain.FindChild<StackPanel>(p_c.Name);
                if (spanel != null)
                {
                    spanel.Children.Remove(exp);
                    Helper.Global.Categorys.Remove(p_c);
                    Helper.Global.SaveCategorys();
                }
            }

        }

        void mitem_upd_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            var link = img.Tag as AppLink;
            if (link != null)
            {
                switch (link.AppType)
                {
                    case LinkType.Sys:
                        System.Diagnostics.Process.Start(link.FileName, link.Args);
                        break;
                    case LinkType.App:
                        System.Diagnostics.Process.Start(link.FileName, link.Args);
                        break;
                    case LinkType.Web:
                        System.Diagnostics.Process.Start(link.FileName);
                        break;
                    case LinkType.Oth:
                        break;
                    default:
                        break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Flyout_Add.IsOpen = true;
        }

        private void btn_AddSubC_Click(object sender, RoutedEventArgs e)
        {
            var subc = new RootCategory() { ID = Guid.NewGuid(), Name = txt_SubCName.Text, PID = (Guid)cbx_root.SelectedValue, SortNum = 1 };
            Helper.Global.Categorys.Add(subc);
            Helper.Global.SaveCategorys();


            var stackPanel = this.tabMain.FindChild<StackPanel>(cbx_root.Text);
            var expander = new Expander() { Name = subc.Name, Header = subc.Name, IsExpanded = true };
            expander.Tag = subc;
            var wrapPanel = new WrapPanel();
            expander.Content = wrapPanel;
            stackPanel.Children.Add(expander);


            txt_SubCName.Text = "";
            Flyout_Add.IsOpen = false;
        }

    }
}
