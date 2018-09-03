using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuHunYYS
{
    public static class Storage
    {
        public const string
            FileNameData = "Yuhun.json",
            FileNameSettings = "Settings.json";

        public static readonly string SettingsPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                App.AppName);

        public static void CreateFolder()
        {
            CreateFolder(SettingsPath);
        }


        public static void CreateFolder(string st)
        {
            if (!Directory.Exists(st))
                System.IO.Directory.CreateDirectory(st);
        }

        public static string GetFileDirectory(string fn)
        {
            var file = Path.Combine(SettingsPath, fn);
            return file;
        }

        public static void Delete(string id)
        {
            try
            {
                var file = Path.Combine(SettingsPath, id + ".json");
                File.Delete(file);
            }
            catch { }
        }

        public static bool Store(string filename, object data)
        {
            try
            {
                CreateFolder(SettingsPath);
                var file = Path.Combine(SettingsPath, filename);
                var json = JsonConvert.SerializeObject(data);

                File.WriteAllText(file, json);
                return true;
            }
            catch { return false; }
        }
        public static T Get<T>(string fileName)
        {
            var file = Path.Combine(SettingsPath, fileName);
            try
            {
                var st = File.ReadAllText(file);
                var ob = JsonConvert.DeserializeObject<T>(st);
                return ob;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
