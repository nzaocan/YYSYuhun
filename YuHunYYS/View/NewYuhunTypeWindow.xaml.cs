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
    /// NewYuhunTypeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewYuhunTypeWindow : Window
    {
        public NewYuhunTypeWindow()
        {
            InitializeComponent();
            App.Settings.YuHunTypeTemp = new YuHunType();
            this.DataContext = App.Settings.YuHunTypeTemp;
            shuxingCombo.ItemsSource = App.Settings.ShuxingList;
        }

        private void OKButton(object sender, RoutedEventArgs e)
        {
            App.Settings.NewYuHunType();
            this.Close();
        }
    }
}
