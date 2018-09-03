using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YuHunYYS.View;

namespace YuHunYYS
{
    /// <summary>
    /// AllShishenWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AllShishenWindow : Window
    {
        public AllShishenWindow()
        {
            InitializeComponent();
            this.DataContext = App.Settings;
        }

        private void NewShiShen(object sender, RoutedEventArgs e)
        {
            var w = new NewShishenWindow();
            w.Owner = this;
            w.ShowDialog();
        }

        private void DeleteSelectedShiShen(object sender, RoutedEventArgs e)
        {
            if (!App.BeforeImportantOP("点击确认将删除选取项")) return;
            var l = new List<Shishen>();
            foreach (var x in UIDG.SelectedItems)
            {
                var item = x as Shishen;
                l.Add(item);
            }
            foreach(var item in l)
                App.Settings.ShishenList.Remove(item);
        }
    }
}
