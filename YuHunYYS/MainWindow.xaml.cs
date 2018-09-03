using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuHunYYS.View;

namespace YuHunYYS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow :UWPWindow
    {
        public ObservableCollection<Limit> Limits { get; set; }
        public List<YuHun> YuhunFangAn { get; set; }
        public Shishen SelectedShishen { get; set; }
        public YuHunType SelectedYuHunType { get; set; }
        public string GoalText { get; set; }

        public DataBase db { get { return App.db; } }
        
        public AppSettings settings
        {
            get { return App.Settings; }
            set { App.Settings = value; }
        }

        public bool IsSijiantaoEnable { get { return !issanjian; } }
        bool issanjian = false;
        public bool IsSanjian
        {
            get { return issanjian; }
            set
            {
                issanjian = value;
                sijiantaoComboBox.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            GoalText = "攻击*爆伤";
            var l = new Limit() { Name = "暴击", Range = ">100" };
            var l1 = new Limit() { Name = "速度", Range = "<128" };
            var l2 = new Limit() { Name = "效果命中", Range = "30-100" };
            Limits = new ObservableCollection<Limit>();
            Limits.Add(l);
            Limits.Add(l1); Limits.Add(l2);

            if (App.Settings.YuhunTypeList.Any(x => x.Name == "针女" && x.ColorR > 0))
            { }
            else
            {
                import();
            }
            App.MW = this;
            this.DataContext = this;
        }

        private void NewShishen(object sender, RoutedEventArgs e)
        {
            var w = new NewShishenWindow();
            w.Owner = this;
            w.ShowDialog();
        }

        private void NewYuhu(object sender, RoutedEventArgs e)
        {
            var w = new NewYuhunWindow();
            w.Owner = this;
            w.ShowDialog();
        }

        private void ManageYuhun(object sender, RoutedEventArgs e)
        {
            var w = new AllYuhunWindow();
            w.Owner = this;
            w.Show();
        }

        private void NewYuhuType(object sender, RoutedEventArgs e)
        {
            var w = new NewYuhunTypeWindow();
            w.Owner = this;
            w.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Save();
        }


        bool isequal(YuHun y1, YuHun y2)
        {
            if (y1.TypeName != y2.TypeName) return false;
            if (y1.Index != y2.Index)
                return false;
            for (int i = 0; i < 5; i++)
            {
                if (y1[i].Name != y2[i].Name ||
                    Math.Abs(y1[i].Val - y2[i].Val) > 0.005)
                    return false;

            }
            return true;
        }

        List<string> splitString(string st)
        {
            try
            {
                var l = st.Split('\t').ToList();
                l.RemoveAll(x => string.IsNullOrWhiteSpace(x));
                return l;
            }
            catch { return null; }
        }
        private void ImportFromFile(object sender, RoutedEventArgs e)
        {
            if (!App.BeforeImportantOP("点击确定完成导入")) return;
            import();
        }

        void import()
        {
            var ft = "类型.txt";
            var f = "御魂.txt";
            var fs = "式神.txt";

            var t = File.ReadAllText(f);
            var tt = File.ReadAllText(ft);
            var ts = File.ReadAllText(fs);


            var sslist = App.Settings.ShishenList;
            foreach (var x in ts.Split(new char[] { '\r', '\n' }))
            {
                if (string.IsNullOrWhiteSpace(x)) continue;
                var a = splitString(x);
                if (a == null) continue;

                if (sslist.Any(xx => xx.Name == a[0])) continue;

                var ss = new Shishen()
                {
                    Name = a[0],
                    Gongji = Convert.ToInt32(a[1]),
                    Shengming = Convert.ToInt32(a[2]),
                    Fangyu = Convert.ToInt32(a[3]),
                    Sudu = Convert.ToInt32(a[4]),
                    Baoji = Convert.ToInt32(a[5]),
                    Baoshang = Convert.ToInt32(a[6]),
                    Mingzhong = Convert.ToInt32(a[7]),
                    Dikang = Convert.ToInt32(a[8]),
                };
                sslist.Add(ss);
            }


            var tlist = App.Settings.YuhunTypeList;
            foreach (var x in tt.Split(new char[] { '\r', '\n' }))
            {
                if (string.IsNullOrWhiteSpace(x)) continue;

                var a = splitString(x);
                if (a == null) continue;

                if (tlist.Any(xx => xx.Name == a[0]))
                {
                    var re = tlist.First(yy => yy.Name == a[0]);
                    re.SetColor(a[1]);
                    continue;
                }

                var yt = new YuHunType();
                yt.Name = a[0];
                yt.SetColor(a[1]);
                yt.Note = a[2];
                yt.Val = Convert.ToInt32(a[3]);
                yt.ShuxingName = a[4];
                tlist.Add(yt);
            }

            var yl = App.db.YuhunList;
            foreach (var x in t.Split(new char[] { '\r', '\n' }))
            {
                if (string.IsNullOrWhiteSpace(x)) continue;
                var a = splitString(x);
                if (a == null) continue;


                var yh = new YuHun();
                yh.Index = Convert.ToInt32(a[0]) - 1;
                yh.TypeName = a[1];
                for (int i = 0; i < 5; i++)
                    yh.Shuxings[i].Name = a[2 + i];

                for (int i = 1; i < 5; i++)
                    yh.Shuxings[i].Val = Convert.ToInt32(a[6 + i]) / 10.0;

                int ind = Convert.ToInt32(a[11]) - 1;
                var sxname = yh.Shuxings[0].Name;

                if (AppSettings.ZhushuxingDict.ContainsKey(sxname))
                {
                    var d = AppSettings.ZhushuxingDict[sxname];
                    yh.Shuxings[0].Val = d[ind];
                }
                else
                {
                    yh.Shuxings[0].Val = 55;
                }

                if (yl.Any(xx => isequal(xx, yh))) continue;
                yh.Type = App.Settings.YuhunTypeList.First(xx => xx.Name == yh.TypeName);
                yl.Add(yh);
            }

        }

        private void LimitRemoveButton(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).Tag as Limit;
            Limits.Remove(item);
        }

        private void NewLimit(object sender, RoutedEventArgs e)
        {
            var l = new Limit();
            Limits.Add(l);
        }

        void beginAnimation()
        {
            UIGrid.IsEnabled = false;
            ProgressRingGrid.Visibility = Visibility.Visible;
        }
        void endAnimation()
        {
            UIGrid.IsEnabled = true;
            ProgressRingGrid.Visibility = Visibility.Collapsed;
        }

        YuhunFangan tempYuhunfangan = null;
        private async void jisuanButton(object sender, RoutedEventArgs e)
        {
            if (SelectedShishen == null || string.IsNullOrWhiteSpace(GoalText))
                return;
            try
            {
                beginAnimation();
                var sjt = SelectedYuHunType;
                if (IsSanjian) sjt = null;
                tempYuhunfangan = await Task.Factory.StartNew(() => App.db.GetYuhunFangan(Limits.ToList(), SelectedShishen, GoalText, sjt));
                endAnimation();
                if (tempYuhunfangan == null)
                {
                    ResultTextBox.Text = "没有满足条件的御魂组合";
                    UIResult.ItemsSource = null;
                    return;
                }
                UIResult.ItemsSource = null;
                UIResult.ItemsSource = tempYuhunfangan.YuhunList;
                ResultTextBox.Text = tempYuhunfangan.ResultText;
            }
            catch
            {
                ResultTextBox.Text = "没有满足条件的御魂组合";
                UIResult.ItemsSource = null;
                endAnimation();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var w = new AllShishenWindow();
            w.Owner = this;
            w.ShowDialog();
        }

        private void NewYuhunTypeWindowClick(object sender, RoutedEventArgs e)
        {
            var w = new AllYuhunTypeWindow();
            w.Owner = this;
            w.ShowDialog();
        }

        private void SaveFanganClick(object sender, RoutedEventArgs e)
        {
            if (tempYuhunfangan == null) return;
            if (App.Settings.YuhunFanganList.Count > 30)
            {
                MessageBox.Show("至多存在30套方案");
                return;
            }
            FanganNamePop.IsOpen = true;
        }

        private void newYuhunFanganClick(object sender, RoutedEventArgs e)
        {
            var s = FanganTextBox.Text.Trim();
            tempYuhunfangan.Name = s;
            if (App.Settings.IsYuhunFanganExsits(tempYuhunfangan))
            {
                MessageBox.Show("方案或者方案名字已存在");
                return;
            }
            App.Settings.YuhunFanganList.Add(tempYuhunfangan);
            FanganNamePop.IsOpen = false;
            MessageBox.Show("保存成功");
        }

        private void ManageYuhunfangan(object sender, RoutedEventArgs e)
        {
            var w = new AllYuhunfanganWindow();
            w.Show();
        }

        private void HideInfoClick(object sender, RoutedEventArgs e)
        {
            infoGrid.Visibility = Visibility.Collapsed;
            UIGrid.Visibility = Visibility.Visible;

        }

        public void ShowInfo()
        {
            infoGrid.Visibility = Visibility.Visible;
            UIGrid.Visibility = Visibility.Collapsed;
        }

        private void MailToNzaocan(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto:nzaocan@icloud.com");
        }
    }
    public class Limit
    {
        public string Name { get; set; }
        public double left, right;
        string range;
        public string Range
        {
            get { return range; }
            set
            {
                range = value;
                try
                {
                    if (range.Contains('-'))
                    {
                        var a = range.Split('-');
                        left = Convert.ToDouble(a[0].Trim());
                        right = Convert.ToDouble(a[1].Trim());
                    }
                    else if (range.Contains('>'))
                    {
                        var ts=range.Replace(">", "");
                        left = Convert.ToDouble(ts.Trim());
                        right = 100000;
                    }
                    else if (range.Contains('<'))
                    {
                        var ts = range.Replace("<", "");
                        right = Convert.ToDouble(ts.Trim());
                        left = 0;
                    }
                }
                catch { }
            }
        }

        public override string ToString()
        {
            return Name + range;
        }
    }
}
