using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// AllYuhunWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AllYuhunWindow : Window
    {
        public ICollectionView view;
        public AllYuhunWindow()
        {
            InitializeComponent();
            this.DataContext = App.db;
            view = CollectionViewSource.GetDefaultView(App.db.YuhunList);
            view.Filter = YuhunFilter;
        }

        private void NewYuhun(object sender, RoutedEventArgs e)
        {
            var w = new NewYuhunWindow();
            w.Show();
        }

        private void DeleteSelectedYuhun(object sender, RoutedEventArgs e)
        {
            var b = App.BeforeImportantOP("点击确定将会删除选中御魂(背景色深色)");
            if (!b) return;
            var l = new List<YuHun>();
            foreach (var x in UIDG.SelectedItems)
            {
                var i = x as YuHun;
                l.Add(i);
            }
            App.db.RemoveYuhun(l);
        }

        void usingSelect()
        {
            foreach (var x in UIDG.SelectedItems)
            {
                var i = x as YuHun;
                i.IsUsing = true;
            }

        }

        private void using_select_click(object sender, RoutedEventArgs e)
        {
            usingSelect();
        }

        private void UsingAllClick(object sender, RoutedEventArgs e)
        {
            foreach (var x in view)
            {
                var i = x as YuHun;
                i.IsUsing = true;
            }
        }

        private void UsingNoneClick(object sender, RoutedEventArgs e)
        {
            foreach (var x in view)
            {
                var i = x as YuHun;
                i.IsUsing = false;
            }
        }

        string serachtext = "";
        private bool YuhunFilter(object o)
        {
            try
            {
                if (string.IsNullOrEmpty(serachtext))
                    return true;
                var w = o as YuHun;

                return w.Display.Contains(serachtext) || w.TypeName.Contains(serachtext) || w.Weizhi == serachtext;
            }
            catch { return true; }
        }

        private void SearchTextChanged(object sender, TextChangedEventArgs e)
        {
            serachtext = (sender as TextBox).Text;
            CollectionViewSource.GetDefaultView(App.db.YuhunList).Refresh();
        }
    }
}
