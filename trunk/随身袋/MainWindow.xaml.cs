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
using MahApps.Metro.Controls;
using 随身袋.Models;
using System.Collections.ObjectModel;
using 随身袋.MyControls;
using System.Globalization;
using 随身袋.Helper;

namespace 随身袋
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        SREngine SRE;
        public MainWindow()
        {
            InitializeComponent();

            Helper.Global.Init();

            Init();

            try
            {
                SRE = new SREngine("随身袋", new string[] { "出来", "退下" });
                SRE.SpeRecSay += SRE_SpeRecSay;
            }
            catch (Exception ex)
            {
                MBox.Show(ex.Message, this);
            }
        }

        void SRE_SpeRecSay(string saytext)
        {
            switch (saytext)
            {
                case "随身袋出来":
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                    {
                        this.Show();
                    }));

                    break;
                case "随身袋退下":
                    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                    {
                        this.Hide();
                    }));

                    break;
            }
        }

        #region 系统托盘
        bool IsCanClose = false;
        System.Windows.Forms.NotifyIcon notifyIcon;
        void Init()
        {
            this.Closing += MainWindow_Closing;

            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.BalloonTipText = "你好, 欢迎使用随身袋!";
            this.notifyIcon.Text = "随身袋!";
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.FriendlyName);
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new EventHandler(delegate { this.Show(); });

            this.notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem closeItem = new System.Windows.Forms.MenuItem("退出");
            closeItem.Click += new EventHandler(delegate { IsCanClose = true; this.Close(); });



            this.notifyIcon.ContextMenu.MenuItems.Add(closeItem);
            this.notifyIcon.ShowBalloonTip(1000);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsCanClose)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
        #endregion

        #region 类别菜单


        ContextMenu SubCMenu = new ContextMenu();

        /// <summary>
        /// 初始化类别菜单
        /// </summary>
        void SubCMenuInit()
        {
            #region 类别菜单
            var mitem_filelink = new MenuItem() { Header = "添加快捷方式" };
            mitem_filelink.Click += mitem_filelink_Click;
            var mi_import = new MenuItem() { Header = "导入快捷方式" };
            mi_import.Click += mi_import_Click;

            var mitem_upd = new MenuItem() { Header = "修改类别" };
            mitem_upd.Click += mitem_upd_Click;
            var mitem_mov = new MenuItem() { Header = "移动类别" };
            mitem_mov.Click += mitem_mov_Click;
            var mitem_del = new MenuItem() { Header = "删除类别" };
            mitem_del.Click += mitem_del_Click;

            SubCMenu.Items.Add(mitem_filelink);
            SubCMenu.Items.Add(mi_import);
            SubCMenu.Items.Add(mitem_upd);
            SubCMenu.Items.Add(mitem_mov);
            SubCMenu.Items.Add(new Separator());
            SubCMenu.Items.Add(mitem_del);
            #endregion
        }

        void mi_import_Click(object sender, RoutedEventArgs e)
        {
            var exp = SubCMenu.PlacementTarget as Expander;
            btn_Import.Tag = exp.Tag;

            var dir = new System.IO.DirectoryInfo(Helper.Global.AppBagName);
            if (dir.Exists)
            {
                var cur_dirs = dir.GetDirectories();
                cbx_Import.ItemsSource = cur_dirs;
            }
            else
            {
                cbx_Import.ItemsSource = null;
            }
            
            Flyout_Import.IsOpen = true;
        }

        /// <summary>
        /// 添加快捷方式,加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mitem_filelink_Click(object sender, RoutedEventArgs e)
        {
            var exp = SubCMenu.PlacementTarget as Expander;

            btn_AddFileLink.Tag = exp.Tag;

            Flyout_AddFileLink.IsOpen = true;
        }

        /// <summary>
        /// 移动操作,加载类别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mitem_mov_Click(object sender, RoutedEventArgs e)
        {
            var exp = SubCMenu.PlacementTarget as Expander;
            cbx_root_Move.ItemsSource = Helper.Global.Categorys.FindAll(m => m.PID == Guid.Empty);
            var c = exp.Tag as RootCategory;
            var p_c = Helper.Global.Categorys.FirstOrDefault(m => m.ID == c.PID);
            cbx_root_Move.SelectedItem = p_c;
            btn_AddSubC_Move.Tag = c.ID;
            Flyout_Move.IsOpen = true;
        }

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mitem_del_Click(object sender, RoutedEventArgs e)
        {
            var exp = SubCMenu.PlacementTarget as Expander;
            var mbr = MBox.Show("确定删除该类别信息么?", "提示", this);
            if (mbr == true)
            {
                var c = exp.Tag as RootCategory;
                var p_c = Helper.Global.Categorys.FirstOrDefault(m => m.ID == c.PID);
                var spanel = this.tabMain.FindChild<StackPanel>(p_c.Name);//查找可见的子控件
                if (spanel != null)
                {
                    spanel.Children.Remove(exp);
                    Helper.Global.Categorys.Remove(c);
                    Helper.Global.SaveCategorys();
                }
            }

        }

        /// <summary>
        /// 修改操作,加载类别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mitem_upd_Click(object sender, RoutedEventArgs e)
        {
            var exp = SubCMenu.PlacementTarget as Expander;
            cbx_root_edit.ItemsSource = Helper.Global.Categorys.FindAll(m => m.PID == Guid.Empty);
            var c = exp.Tag as RootCategory;
            var p_c = Helper.Global.Categorys.FirstOrDefault(m => m.ID == c.PID);
            cbx_root_edit.SelectedItem = p_c;
            txt_SubCName_edit.Tag = c.ID;
            txt_SubCName_edit.Text = c.Name;
            nud_SortNum_edit.Value = c.SortNum;
            Flyout_Edit.IsOpen = true;

        }
        #endregion

        #region 快捷方式菜单

        ContextMenu SubCMenu_App = new ContextMenu();

        /// <summary>
        /// 初始化快捷方式菜单
        /// </summary>
        void SubCMenu_AppInit()
        {
            #region 快捷方式菜单
            var item1 = new MenuItem() { Header = "打开" };

            var item2 = new MenuItem() { Header = "修改" };

            var item3 = new MenuItem() { Header = "移动" };

            var item4 = new MenuItem() { Header = "删除" };

            SubCMenu_App.Items.Add(item1);
            SubCMenu_App.Items.Add(item2);
            SubCMenu_App.Items.Add(item3);
            SubCMenu_App.Items.Add(item4);
            #endregion
        }

        #endregion


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SubCMenuInit();
            SubCMenu_AppInit();


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

                    wrapPanel.ContextMenu = SubCMenu_App;
                    foreach (var link in Helper.Global.AppLinks.FindAll(m => m.PID == subc.ID))
                    {
                        wrapPanel.Children.Add(GetImg(link));
                    }

                    expander.Content = wrapPanel;
                    stackPanel.Children.Add(expander);
                }

                tabItem.Content = stackPanel;
                tabMain.Items.Add(tabItem);
            }




        }

        /// <summary>
        /// 生成打包好的图片对象
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        Border GetImg(AppLink link)
        {
            var border = new Border() { Name = link.Name, Style = (Style)this.FindResource("LinkBorder") };
            var label = new Label() { Width = 64, Height = 64, Style = (Style)this.FindResource("LinkLabel"), HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center, VerticalContentAlignment = System.Windows.VerticalAlignment.Center };
            var image = new Image()
            {
                ToolTip = link.Name,
                Width = 32,
                Height = 32,
            };
            if (link.IsRelative)
            {
                image.Source = Helper.Global.GetIcon(Helper.Global.AppPath + link.FileName);
            }
            else
            {
                image.Source = Helper.Global.GetIcon(link.FileName);
            }
            image.Tag = link;
            image.MouseLeftButtonDown += image_MouseLeftButtonDown;
            label.Content = image;
            border.Child = label;
            return border;
        }

        /// <summary>
        /// 单击打开链接文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as Image;
            var link = img.Tag as AppLink;
            if (link != null)
            {
                if (link.IsRelative == true)
                {
                    System.Diagnostics.Process.Start(System.IO.Path.Combine(Helper.Global.AppPath, link.FileName), link.Args);
                }
                else
                {
                    System.Diagnostics.Process.Start(link.FileName, link.Args);
                }

            }
        }

        /// <summary>
        /// 添加类别,加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Flyout_Add.IsOpen = true;
        }

        /// <summary>
        /// 添加类别,操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddSubC_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_SubCName.Text)) { txt_SubCName.Focus(); return; }
            var only = Helper.Global.Categorys.FirstOrDefault(m => m.Name.ToUpper() == txt_SubCName.Text.ToUpper());
            if (only != null)
            {
                txt_SubCName.Text = "";
                TextBoxHelper.SetWatermark(txt_SubCName, "名称已存在!");
                return;
            }
            var subc = new RootCategory() { ID = Guid.NewGuid(), Name = txt_SubCName.Text, PID = (Guid)cbx_root.SelectedValue, SortNum = (int)nud_SortNum.Value };
            Helper.Global.Categorys.Add(subc);
            Helper.Global.SaveCategorys();


            var stackPanel = this.tabMain.FindChildren<StackPanel>().FirstOrDefault(m => m.Name == cbx_root.Text);//查找所有的子控件
            var expander = new Expander() { Name = subc.Name, Header = subc.Name, IsExpanded = true };
            expander.Tag = subc;
            expander.ContextMenu = SubCMenu;
            var wrapPanel = new WrapPanel();
            wrapPanel.ContextMenu = SubCMenu_App;
            expander.Content = wrapPanel;
            stackPanel.Children.Add(expander);


            txt_SubCName.Text = "";
            Flyout_Add.IsOpen = false;
        }

        /// <summary>
        /// 更新类别,操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddSubC_edit_Click(object sender, RoutedEventArgs e)
        {
            //此方法,暂不支持更换类别
            var subc = Helper.Global.Categorys.FirstOrDefault(m => m.ID == ((Guid)txt_SubCName_edit.Tag));

            var expander = this.tabMain.FindChild<Expander>(subc.Name);//查找可见的子控件

            subc.Name = txt_SubCName_edit.Text;
            subc.PID = (Guid)cbx_root_edit.SelectedValue;
            subc.SortNum = Convert.ToInt32(nud_SortNum_edit.Value);

            expander.Name = subc.Name;
            expander.Header = subc.Name;
            expander.Tag = subc;
            expander.ContextMenu = SubCMenu;

            Helper.Global.SaveCategorys();

            Flyout_Edit.IsOpen = false;
        }

        /// <summary>
        /// 移动类别,操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddSubC_Move_Click(object sender, RoutedEventArgs e)
        {
            var id = (Guid)btn_AddSubC_Move.Tag;
            var subc = Helper.Global.Categorys.FirstOrDefault(m => m.ID == id);
            subc.PID = (Guid)cbx_root_Move.SelectedValue;

            var stackPanel_temp = this.tabMain.SelectedContent as StackPanel;//查找可见的子控件
            var stackPanel = this.tabMain.FindChildren<StackPanel>().FirstOrDefault(sp => sp.Name == cbx_root_Move.Text); //查找所有子控件
            var expander = this.tabMain.FindChild<Expander>(subc.Name);
            stackPanel_temp.Children.Remove(expander);
            stackPanel.Children.Add(expander);

            Helper.Global.SaveCategorys();
            Flyout_Move.IsOpen = false;
        }

        /// <summary>
        /// 添加快捷方式,操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddFileLink_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_LinkName.Text)) { txt_LinkName.Focus(); return; }
            if (string.IsNullOrWhiteSpace(txt_LinkFileName.Text)) { txt_LinkFileName.Focus(); return; }
            var only = Helper.Global.AppLinks.FirstOrDefault(m => m.Name.ToUpper() == txt_LinkName.Text.ToUpper());
            if (only != null)
            {
                txt_LinkName.Text = "";
                TextBoxHelper.SetWatermark(txt_LinkName, "名称已存在!");
                return;
            }

            var subc = btn_AddFileLink.Tag as RootCategory;
            var link = new AppLink() { ID = Guid.NewGuid(), Name = txt_LinkName.Text, FileName = txt_LinkFileName.Text, Args = txt_Args.Text, IsRelative = chb_IsRelative.IsChecked == true, Tags = txt_Tags.Text, SortNum = (int)nud_Sort.Value, PID = subc.ID };

            Helper.Global.AppLinks.Add(link);
            Helper.Global.SaveAppLinks();

            var wrapPanel = this.tabMain.FindChild<Expander>(subc.Name).Content as WrapPanel;
            wrapPanel.Children.Add(GetImg(link));

            Flyout_AddFileLink.IsOpen = false;
            txt_LinkName.Text = "";
            txt_LinkFileName.Text = "";
            btn_AddFileLink.Tag = null;
            txt_Args.Text = "";
            chb_IsRelative.IsChecked = true;
            txt_Tags.Text = "";
            nud_Sort.Value = 1;
        }

        /// <summary>
        /// 获取文件路径名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_LinkFileName_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "执行文件|*.exe|所有文件|*.*",
            };
            if (chb_IsRelative.IsChecked == true)
            {
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                openFileDialog.InitialDirectory = "C:\\";
            }
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (chb_IsRelative.IsChecked == true)
                {
                    txt_LinkFileName.Text = openFileDialog.FileName.Replace(AppDomain.CurrentDomain.BaseDirectory, "");
                }
                else
                {
                    txt_LinkFileName.Text = System.IO.Path.GetFullPath(openFileDialog.FileName);
                }
                txt_LinkName.Text = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //搜索
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txt_Import_TextChanged(object sender, TextChangedEventArgs e)
        {
            var dir = new System.IO.DirectoryInfo(System.IO.Path.Combine(Helper.Global.AppBagName,txt_Import.Text));
            if(dir.Exists)
            {
                var cur_dirs = dir.GetDirectories();
                cbx_Import.ItemsSource = cur_dirs;
            }
            else
            {
                cbx_Import.ItemsSource = null;
            }
        }


        private void cbx_Import_DropDownClosed(object sender, EventArgs e)
        {
            txt_Import.Text = System.IO.Path.Combine(txt_Import.Text, cbx_Import.Text);
        }



    }
}
