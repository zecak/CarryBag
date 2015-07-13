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
using System.Windows.Threading;

namespace 随身袋
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        SREngine SRE;
        DispatcherTimer autoTimer;
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var autoHide = new WindowAutoHide(this);

                var windowSysMin = new WindowSysMin(this);


                autoTimer = new DispatcherTimer();
                autoTimer.Interval = TimeSpan.FromMilliseconds(1000*30);
                autoTimer.Tick += autoTimer_Tick;
                autoTimer.Start();

                Helper.Global.Init();

                ////语音控制
                //try
                //{
                //    SRE = new SREngine("随身袋", new string[] { "出来", "退下" });
                //    SRE.SpeRecSay += SRE_SpeRecSay;
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}

                加载类别();

                //文件保护
                //System.IO.Directory.SetAccessControl(@"D:\登录", new System.Security.AccessControl.DirectorySecurity("hh", System.Security.AccessControl.AccessControlSections.Audit));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        void SysRun(bool enable)
        {
            Microsoft.Win32.RegistryKey HKCU = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey Run = HKCU.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            try
            {
                var exe = new System.IO.FileInfo(AppDomain.CurrentDomain.FriendlyName);
                if (enable)
                {
                    Run.SetValue("CarryBag", exe.FullName);
                }
                else
                {
                    Run.DeleteValue("CarryBag");
                }
            }
            catch { }
            HKCU.Close();
        }


        void autoTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                lblTime.ToolTip = "农历" + NLCalendar.GetCalendar(DateTime.Now);
                lblTime.Content = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek) + " " + DateTime.Now.ToLongDateString();

            }
            catch { }
            
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

        void 加载类别()
        {
            SubCMenuInit();
            SubCMenu_AppInit();


            cbx_root.ItemsSource = Helper.Global.Categorys.FindAll(m => m.PID == Guid.Empty).OrderBy(m => m.SortNum);
            foreach (var c in Helper.Global.Categorys.FindAll(m => m.PID == Guid.Empty).OrderBy(m => m.SortNum))
            {
                var tabItem = new TabItem() { Header = c.Name };
                tabItem.Tag = c;
                ControlsHelper.SetHeaderFontSize(tabItem, 18);

                var scrollViewer = new ScrollViewer();
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                var stackPanel = new StackPanel() { Name = Helper.Global.EncodeCtrlName(c.ID.ToString()) };

                foreach (var subc in Helper.Global.Categorys.FindAll(m => m.PID == c.ID).OrderBy(m => m.SortNum))
                {
                    var expander = new Expander() { Name = Helper.Global.EncodeCtrlName(subc.ID.ToString()), Header = subc.Name, IsExpanded = true };
                    expander.Tag = subc;

                    expander.ContextMenu = SubCMenu;

                    var wrapPanel = new ListBox();

                    //Helper.ListBoxSelector.SetEnabled(wrapPanel, true);


                    foreach (var link in Helper.Global.AppLinks.FindAll(m => m.PID == subc.ID).OrderBy(m => m.SortNum))
                    {
                        wrapPanel.Items.Add(GetImg(link));
                    }



                    expander.Content = wrapPanel;
                    stackPanel.Children.Add(expander);
                }

                scrollViewer.Content = stackPanel;

                tabItem.Content = scrollViewer;
                tabMain.Items.Add(tabItem);
            }

        }

        #region 类别菜单


        ContextMenu SubCMenu = new ContextMenu();

        /// <summary>
        /// 初始化栏目菜单
        /// </summary>
        void SubCMenuInit()
        {
            #region 类别菜单
            var mitem_filelink = new MenuItem() { Header = "添加快捷方式" };
            mitem_filelink.Click += mitem_filelink_Click;
            var mi_import = new MenuItem() { Header = "导入快捷方式" };
            mi_import.Click += mi_import_Click;

            var mitem_upd = new MenuItem() { Header = "修改栏目" };
            mitem_upd.Click += mitem_upd_Click;
            var mitem_mov = new MenuItem() { Header = "移动栏目" };
            mitem_mov.Click += mitem_mov_Click;
            var mitem_del = new MenuItem() { Header = "删除栏目" };
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
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 添加快捷方式,加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mitem_filelink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exp = SubCMenu.PlacementTarget as Expander;

                btn_AddFileLink.Tag = exp.Tag;

                Flyout_AddFileLink.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 移动操作,加载栏目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mitem_mov_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exp = SubCMenu.PlacementTarget as Expander;
                cbx_root_Move.ItemsSource = Helper.Global.Categorys.FindAll(m => m.PID == Guid.Empty).OrderBy(m => m.SortNum);
                var c = exp.Tag as RootCategory;
                var p_c = Helper.Global.Categorys.FirstOrDefault(m => m.ID == c.PID);
                cbx_root_Move.SelectedItem = p_c;
                btn_AddSubC_Move.Tag = c.ID;
                Flyout_Move.Header = "移动[" + c.Name + "]到";
                Flyout_Move.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mitem_del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exp = SubCMenu.PlacementTarget as Expander;
                var mbr = MBox.Show("确定删除该栏目信息么?", "提示", this);
                if (mbr == true)
                {
                    var c = exp.Tag as RootCategory;
                    var p_c = Helper.Global.Categorys.FirstOrDefault(m => m.ID == c.PID);
                    var spanel = this.tabMain.FindChild<StackPanel>(Helper.Global.EncodeCtrlName(p_c.ID.ToString()));//查找可见的子控件
                    if (spanel != null)
                    {
                        spanel.Children.Remove(exp);

                        Helper.Global.AppLinks.RemoveAll(m => m.PID == c.ID);
                        Helper.Global.SaveAppLinks();
                        Helper.Global.Categorys.Remove(c);
                        Helper.Global.SaveCategorys();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 修改操作,加载类别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mitem_upd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exp = SubCMenu.PlacementTarget as Expander;
                cbx_root_edit.ItemsSource = Helper.Global.Categorys.FindAll(m => m.PID == Guid.Empty).OrderBy(m => m.SortNum);
                var c = exp.Tag as RootCategory;
                var p_c = Helper.Global.Categorys.FirstOrDefault(m => m.ID == c.PID);
                cbx_root_edit.SelectedItem = p_c;
                txt_SubCName_edit.Tag = c.ID;
                txt_SubCName_edit.Text = c.Name;
                nud_SortNum_edit.Value = c.SortNum;
                Flyout_Edit.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            var item_open = new MenuItem() { Header = "打开" };
            item_open.Click += item_open_Click;

            var item_update = new MenuItem() { Header = "修改" };
            item_update.Click += item_update_Click;

            var item_move = new MenuItem() { Header = "移动" };
            item_move.Click += item_move_Click;

            var item_del = new MenuItem() { Header = "删除" };
            item_del.Click += item_del_Click;

            SubCMenu_App.Items.Add(item_open);
            SubCMenu_App.Items.Add(item_update);
            //SubCMenu_App.Items.Add(item_move);
            SubCMenu_App.Items.Add(new Separator());
            SubCMenu_App.Items.Add(item_del);
            #endregion
        }

        void item_del_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var listbox = (SubCMenu_App.PlacementTarget as Border).Parent as ListBox;
                var list = listbox.SelectedItems.OfType<Border>().ToList();
                foreach (var b in list)
                {
                    var link = b.Tag as AppLink;
                    if (link != null)
                    {
                        Global.AppLinks.Remove(link);
                        listbox.Items.Remove(b);
                    }
                }
                Global.SaveAppLinks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void item_move_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void item_update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var listbox = (SubCMenu_App.PlacementTarget as Border).Parent as ListBox;
                var border = listbox.SelectedItems.OfType<Border>().FirstOrDefault();
                if (border == null) { return; }
                var link = border.Tag as AppLink;
                if (link == null) { return; }

                chb_IsRelativeEdit.IsChecked = link.IsRelative;
                txt_LinkFileNameEdit.Text = link.FileName;
                txt_LinkNameEdit.Text = link.Name;
                txt_ArgsEdit.Text = link.Args;
                txt_TagsEdit.Text = link.Tags;
                nud_SortEdit.Value = link.SortNum;
                btn_EditFileLink.Tag = link;

                Flyout_EditFileLink.IsOpen = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void item_open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var listbox = (SubCMenu_App.PlacementTarget as Border).Parent as ListBox;
                var list = listbox.SelectedItems.OfType<Border>().ToList();
                foreach (var b in list)
                {
                    var link = b.Tag as AppLink;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btn_LinkFileNameEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "执行文件|*.exe|所有文件|*.*",
                };
                if (chb_IsRelativeEdit.IsChecked == true)
                {
                    openFileDialog.InitialDirectory = Helper.Global.AppBagPath;
                }
                else
                {
                    openFileDialog.InitialDirectory = "C:\\";
                }
                var result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    if (chb_IsRelativeEdit.IsChecked == true)
                    {
                        txt_LinkFileNameEdit.Text = openFileDialog.FileName.Replace(AppDomain.CurrentDomain.BaseDirectory, "");
                    }
                    else
                    {
                        txt_LinkFileNameEdit.Text = System.IO.Path.GetFullPath(openFileDialog.FileName);
                    }
                    txt_LinkNameEdit.Text = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    txt_TagsEdit.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_EditFileLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_LinkNameEdit.Text)) { txt_LinkNameEdit.Focus(); return; }
                if (string.IsNullOrWhiteSpace(txt_LinkFileNameEdit.Text)) { txt_LinkFileNameEdit.Focus(); return; }

                var temp_link = btn_EditFileLink.Tag as AppLink;
                var link = Helper.Global.AppLinks.FirstOrDefault(m => m.ID == temp_link.ID);
                link.Name = txt_LinkNameEdit.Text;
                link.FileName = txt_LinkFileNameEdit.Text;
                link.Args = txt_ArgsEdit.Text;
                link.IsRelative = chb_IsRelativeEdit.IsChecked == true;
                link.Tags = txt_TagsEdit.Text;
                link.SortNum = (int)nud_SortEdit.Value;
                link.Extension = System.IO.Path.GetExtension(txt_LinkFileNameEdit.Text);

                Helper.Global.SaveAppLinks();


                var listbox = this.tabMain.FindChild<Expander>(Helper.Global.EncodeCtrlName(link.PID.ToString())).Content as ListBox;
                listbox.Items.Clear();
                foreach (var l in Helper.Global.AppLinks.FindAll(m => m.PID == link.PID).OrderBy(m => m.SortNum))
                {
                    listbox.Items.Add(GetImg(l));
                }


                Flyout_EditFileLink.IsOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion


        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

            //System.IO.Path.GetExtension()

        }

        /// <summary>
        /// 生成打包好的图片对象
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        Border GetImg(AppLink link)
        {
            try
            {
                var border = new Border() { Name = Helper.Global.EncodeCtrlName(link.ID.ToString()), Style = (Style)this.FindResource("LinkBorder"), ToolTip = link.Name + "\r\n" + link.FileName, Tag = link, ContextMenu = SubCMenu_App };
                var label = new Label() { Width = 64, Height = 64, Style = (Style)this.FindResource("LinkLabel"), HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center, VerticalContentAlignment = System.Windows.VerticalAlignment.Center, ToolTip = border.ToolTip, Tag = link };
                var image = new Image()
                {
                    ToolTip = border.ToolTip,
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
                //image.MouseLeftButtonUp += image_MouseLeftButtonDown;
                label.MouseLeftButtonDown += image_MouseLeftButtonDown;
                label.Content = image;
                border.Child = label;
                return border;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new Border();
            }

        }

        void SetImg(Border border, AppLink link)
        {
            border.ToolTip = link.Name + "\r\n" + link.FileName;
            border.Tag = link;
            var label = border.Child as Label;
            label.ToolTip = border.ToolTip;
            label.Tag = link;
            var image = label.Content as Image;
            image.ToolTip = border.ToolTip;
            if (link.IsRelative)
            {
                image.Source = Helper.Global.GetIcon(Helper.Global.AppPath + link.FileName);
            }
            else
            {
                image.Source = Helper.Global.GetIcon(link.FileName);
            }
            image.Tag = link;


        }

        /// <summary>
        /// 单击打开链接文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var img = sender as Image;
                if (img == null) { img = (sender as Label).Content as Image; }
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            try
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


                var stackPanel = this.tabMain.FindChildren<StackPanel>().FirstOrDefault(m => m.Name == Helper.Global.EncodeCtrlName(subc.PID.ToString()));//查找所有的子控件

                //stackPanel.Children.Clear();

                var expander = new Expander() { Name = Helper.Global.EncodeCtrlName(subc.ID.ToString()), Header = subc.Name, IsExpanded = true };
                expander.Tag = subc;
                expander.ContextMenu = SubCMenu;
                var wrapPanel = new ListBox();
                //Helper.ListBoxSelector.SetEnabled(wrapPanel, true);
                expander.Content = wrapPanel;
                stackPanel.Children.Add(expander);


                txt_SubCName.Text = "";
                Flyout_Add.IsOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 更新类别,操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddSubC_edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //此方法,暂不支持更换类别
                var subc = Helper.Global.Categorys.FirstOrDefault(m => m.ID == ((Guid)txt_SubCName_edit.Tag));

                var expander = this.tabMain.FindChild<Expander>(Helper.Global.EncodeCtrlName(subc.ID.ToString()));//查找可见的子控件

                subc.Name = txt_SubCName_edit.Text;
                subc.PID = (Guid)cbx_root_edit.SelectedValue;
                subc.SortNum = Convert.ToInt32(nud_SortNum_edit.Value);

                expander.Name = Helper.Global.EncodeCtrlName(subc.ID.ToString());
                expander.Header = subc.Name;
                expander.Tag = subc;
                expander.ContextMenu = SubCMenu;

                Helper.Global.SaveCategorys();

                Flyout_Edit.IsOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 移动类别,操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddSubC_Move_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = (Guid)btn_AddSubC_Move.Tag;
                var subc = Helper.Global.Categorys.FirstOrDefault(m => m.ID == id);
                subc.PID = (Guid)cbx_root_Move.SelectedValue;

                var stackPanel_temp = this.tabMain.SelectedContent as StackPanel;//查找可见的子控件
                var stackPanel = this.tabMain.FindChildren<StackPanel>().FirstOrDefault(sp => sp.Name == Helper.Global.EncodeCtrlName(subc.PID.ToString())); //查找所有子控件
                var expander = this.tabMain.FindChild<Expander>(Helper.Global.EncodeCtrlName(subc.ID.ToString()));
                stackPanel_temp.Children.Remove(expander);
                stackPanel.Children.Add(expander);

                Helper.Global.SaveCategorys();
                Flyout_Move.IsOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 添加快捷方式,操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddFileLink_Click(object sender, RoutedEventArgs e)
        {
            try
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
                var link = new AppLink() { ID = Guid.NewGuid(), Name = txt_LinkName.Text, FileName = txt_LinkFileName.Text, Args = txt_Args.Text, IsRelative = chb_IsRelative.IsChecked == true, Tags = txt_Tags.Text, SortNum = (int)nud_Sort.Value, PID = subc.ID, Extension = System.IO.Path.GetExtension(txt_LinkFileName.Text) };

                Helper.Global.AppLinks.Add(link);
                Helper.Global.SaveAppLinks();

                var wrapPanel = this.tabMain.FindChild<Expander>(Helper.Global.EncodeCtrlName(subc.ID.ToString())).Content as ListBox;

                wrapPanel.Items.Clear();
                foreach (var l in Helper.Global.AppLinks.FindAll(m => m.PID == link.PID).OrderBy(m => m.SortNum))
                {
                    wrapPanel.Items.Add(GetImg(l));
                }


                Flyout_AddFileLink.IsOpen = false;
                txt_LinkName.Text = "";
                txt_LinkFileName.Text = "";
                btn_AddFileLink.Tag = null;
                txt_Args.Text = "";
                chb_IsRelative.IsChecked = true;
                txt_Tags.Text = "";
                nud_Sort.Value = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 获取文件路径名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_LinkFileName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "执行文件|*.exe|所有文件|*.*",
                };
                if (chb_IsRelative.IsChecked == true)
                {
                    openFileDialog.InitialDirectory = Helper.Global.AppBagPath;
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
                    txt_Tags.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var p_cate = btn_Import.Tag as RootCategory;
                var dir = new System.IO.DirectoryInfo(System.IO.Path.Combine(Helper.Global.AppBagName, txt_Import.Text));
                if (dir.Exists)
                {
                    var wrapPanel = this.tabMain.FindChild<Expander>(Helper.Global.EncodeCtrlName(p_cate.ID.ToString())).Content as ListBox;
                    var files = dir.GetFiles("*.exe", System.IO.SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var link = new AppLink() { ID = Guid.NewGuid(), IsRelative = true, SortNum = 99, Name = System.IO.Path.GetFileNameWithoutExtension(file.FullName), Tags = System.IO.Path.GetFileName(file.FullName), FileName = file.FullName.Replace(AppDomain.CurrentDomain.BaseDirectory, ""), PID = p_cate.ID, Extension = file.Extension };
                        var only = Helper.Global.AppLinks.FirstOrDefault(m => m.Name.ToUpper() == link.Name.ToUpper());
                        if (only == null)
                        {
                            Helper.Global.AppLinks.Add(link);
                            wrapPanel.Items.Add(GetImg(link));
                        }
                    }
                    Helper.Global.SaveAppLinks();
                    Flyout_Import.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txt_Import_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var dir = new System.IO.DirectoryInfo(System.IO.Path.Combine(Helper.Global.AppBagName, txt_Import.Text));
                if (dir.Exists)
                {
                    var cur_dirs = dir.GetDirectories();
                    cbx_Import.ItemsSource = cur_dirs;
                }
                else
                {
                    cbx_Import.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void cbx_Import_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                txt_Import.Text = System.IO.Path.Combine(txt_Import.Text, cbx_Import.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var all_list = this.tabMain.FindChildren<Border>();

                var tbox = sender as TextBox;
                if (string.IsNullOrWhiteSpace(tbox.Text))
                {
                    foreach (var a in all_list.Where(m => m.Visibility != System.Windows.Visibility.Visible))
                    {
                        a.Visibility = System.Windows.Visibility.Visible;
                    }

                    return;
                }

                foreach (var b in all_list)
                {
                    b.Visibility = System.Windows.Visibility.Collapsed;
                }


                var blist = all_list.Where(m => (m.Tag as AppLink).Tags.ToUpper().Contains(tbox.Text.ToUpper()) || (m.Tag as AppLink).Name.ToUpper().Contains(tbox.Text.ToUpper())); //查找所有子控件

                foreach (var b in blist)
                {
                    b.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_about_Click(object sender, RoutedEventArgs e)
        {
            fly_about.IsOpen = true;
        }

        private void ts_SysRun_IsCheckedChanged(object sender, EventArgs e)
        {
            SysRun(ts_SysRun.IsChecked == true);
        }

        private void btn_set_Click(object sender, RoutedEventArgs e)
        {
            fly_set.IsOpen = true;
        }


    }
}
