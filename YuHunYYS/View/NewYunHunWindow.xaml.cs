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

namespace YuHunYYS.View
{
    /// <summary>
    /// NewYunHunWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewYuhunWindow : Window
    {
        YuHun ty { get { return App.Settings.TempYuhun; } }
        public NewYuhunWindow()
        {
            InitializeComponent();
            App.Settings.TempYuhun = new YuHun();
            this.DataContext = App.Settings;
            tick();
        }
        async void tick()
        {
            await Task.Delay(200);
            App.Settings.TempYuhun.Index = 1;
        }

        private async void OKButton(object sender, RoutedEventArgs e)
        {
            if (ty.Type == null)
            {
                MessageBox.Show("没有御魂类型");
                return;
            } 
            App.db.AddYuhun();
            var t = Title;
            Title = "御魂已新增" + DateTime.Now.ToString("t");
            await Task.Delay(2000);
            Title = t;
        }
        
        private void IndexComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var i = IndexComboBox.SelectedIndex;
            if (i < 0) return;
            MainComboBox.ItemsSource = null;
            MainComboBox.ItemsSource = App.Settings.PosibleShuxing[i];
            MainComboBox.SelectedIndex = 0;
        }

        static List<double> p55list = new List<double>() { 55 };
        private void MainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var st=MainComboBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(st)) return;
            ValComboBox.ItemsSource = null;
            if (AppSettings.ZhushuxingDict.ContainsKey(st))
                ValComboBox.ItemsSource = AppSettings.ZhushuxingDict[st];
            else
                ValComboBox.ItemsSource = p55list;
            ValComboBox.SelectedIndex = 0;
        }
    }
}
