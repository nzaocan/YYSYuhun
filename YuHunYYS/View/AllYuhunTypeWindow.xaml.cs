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
    /// AllYuhunTypeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AllYuhunTypeWindow : Window
    {
        public AllYuhunTypeWindow()
        {
            InitializeComponent();
            this.DataContext = App.Settings;

        }

        private void NewYuhunType(object sender, RoutedEventArgs e)
        {
            var w = new NewYuhunTypeWindow();
            w.Owner = this;
            w.ShowDialog();

        }

        private void DeleteSelectedYuhunType(object sender, RoutedEventArgs e)
        {
            if (!App.BeforeImportantOP("点击确认将删除选取项")) return;
            var l = new List<YuHunType>();
            foreach (var x in UIDG.SelectedItems)
            {
                var item = x as YuHunType;
                l.Add(item);
            }

            foreach (var x in l)
                App.Settings.YuhunTypeList.Remove(x);
        }
    }
}
