using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace YuHunYYS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static AppSettings Settings { get; set; }
        public static DataBase db = new DataBase();
        public static string AppName = "阴阳师御魂组合助手";
        public static MainWindow MW { get; set; }

        static private ICommand contactmeCommand;

        public static ICommand ContactmeCommand
        {
            get
            {
                return contactmeCommand ??
                    (contactmeCommand = new SimpleCommand
                    {
                        CanExecuteDelegate = x => true,
                        ExecuteDelegate = x => ContactME()
                    });
            }
        }

        public static void ContactME()
        {
            MW.ShowInfo();
            string mail = "mailto:nzaocan@icloud.com";
            Process.Start(mail);
        }
        public App()
        {
            Settings = Storage.Get<AppSettings>(Storage.FileNameSettings);
            if (Settings == null)
                Settings = new AppSettings();

            db = Storage.Get<DataBase>(Storage.FileNameData);
            if (db == null)
                db = new DataBase();


            //先初始化御魂数据，后初始化方案
            db.Init();
            Settings.Init();
        }

        public static bool BeforeImportantOP(string title)
        {
            return MessageBox.Show(title, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }

        public static void Save()
        {
            Storage.Store(Storage.FileNameSettings, Settings);
            Storage.Store(Storage.FileNameData, db);
        }

        public static YuHunType Getyuhuntype(string name)
        {
            try
            {
                return Settings.YuhunTypeList.First(x => x.Name == name);
            }
            catch
            {
                try
                {
                    return Settings.YuhunTypeList[0];
                }
                catch
                {
                    return new YuHunType();
                }
            }
        }

        public List<string> baifenbiShuxingList = new List<string>()
        {
            "攻击加成","防御加成","生命加成"
        };


    }

    public class SimpleCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
                return CanExecuteDelegate(parameter);
            return true; // if there is no can execute default to true
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
                ExecuteDelegate(parameter);
        }
    }

}
