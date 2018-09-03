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
    /// NewShishenWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewShishenWindow : Window
    {
        public NewShishenWindow()
        {
            InitializeComponent();
            App.Settings.ShishenTemp = new Shishen();
            this.DataContext = App.Settings.ShishenTemp;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Settings.NewShishen();
            var t = Title;
            Title = "式神" + App.Settings.ShishenTemp.Name + "已新增";
            await Task.Delay(2000);
            Title = t;
        }
    }
}
