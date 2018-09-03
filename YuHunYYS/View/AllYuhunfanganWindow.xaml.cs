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

namespace YuHunYYS
{
    /// <summary>
    /// AllYuhunfanganWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AllYuhunfanganWindow : Window
    {
        public AllYuhunfanganWindow()
        {
            InitializeComponent();
            this.DataContext = App.Settings;

        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (!App.BeforeImportantOP("点击确认将删除选取项")) return;
            var l = new List<YuhunFangan>();
            foreach (var x in UIDG.SelectedItems)
            {
                var item = x as YuhunFangan;
                l.Add(item);
            }

            foreach (var x in l)
                App.Settings.YuhunFanganList.Remove(x);
        }
    }
}
