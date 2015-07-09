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
using System.Windows.Shapes;

namespace 随身袋.MyControls
{
    /// <summary>
    /// MBox.xaml 的交互逻辑
    /// </summary>
    public partial class MBox
    {
        public MBox()
        {
            InitializeComponent();
        }

        public string Message
        {
            get { return this.lblMsg.Text; }
            set { this.lblMsg.Text = value; }
        }
        public static bool? Show(string msg,Window win=null)
        {
            var msgBox = new MBox();
            msgBox.Message = msg;
            msgBox.Owner = win;
            return msgBox.ShowDialog();
        }
        public static bool? Show(string msg, string title, Window win = null)
        {
            var msgBox = new MBox();
            msgBox.Title = title;
            msgBox.Message = msg;
            msgBox.Owner = win;
            return msgBox.ShowDialog();
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Canel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
