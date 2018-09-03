using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuHunYYS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AppSettings
    {


        public List<string> ShuxingList { get { return shuxingList; } }
        public List<string> IndexStringList { get { return indexStringList; } }
        public List<string>[] PosibleShuxing { get { return posibleShuxing; } }


        [JsonProperty]
        public ObservableCollection<Shishen> ShishenList { get; set; }
        [JsonProperty]
        public ObservableCollection<YuHunType> YuhunTypeList { get; set; }
        [JsonProperty]
        public ObservableCollection<YuhunFangan> YuhunFanganList { get; set; }

        public Shishen ShishenTemp { get; set; }
        public YuHunType YuHunTypeTemp { get; set; }
        public YuHun TempYuhun { get; set; }

        public bool IsYuhunFanganExsits(YuhunFangan f)
        {
            if (YuhunFanganList.Any(x => x == f || x.Name == f.Name)) return true;
            foreach (var x in YuhunFanganList)
            {
                var b = true;
                foreach (var y in x.Yuhuns)
                {
                    if (!f.Yuhuns.Contains(y))
                        b = false;
                }
                if (b) return true;
            }
            return false;
        }

        public void NewYuHunType()
        {
            YuhunTypeList.Add(YuHunTypeTemp);
        }
        public void NewShishen()
        {
            ShishenList.Add(ShishenTemp);
        }

        public AppSettings()
        {

        }

        public void Init()
        {
            if (ShishenList == null)
                ShishenList = new ObservableCollection<Shishen>();
            if (YuhunTypeList == null)
                YuhunTypeList = new ObservableCollection<YuHunType>();
            if (YuhunFanganList == null)
                YuhunFanganList = new ObservableCollection<YuhunFangan>();

            foreach (var x in YuhunFanganList)
                x.Init();
        }

        public static List<string> indexStringList = new List<string>()
        {
            "一号位",
            "二号位",
            "三号位",
            "四号位",
            "五号位",
            "六号位",
        };

        public static List<string> shuxingList = new List<string>()
        {
            "暴击",
            "暴击伤害",
            "攻击加成",
            "速度",
            "效果抵抗",
            "效果命中",
            "防御加成",
            "生命加成",
            "攻击","防御","生命"
        };
        public static Dictionary<string, int> shuxingchengzhang = new Dictionary<string, int>()
        {
            {"暴击",1503 },
            {"暴击伤害" ,1004},
            {"攻击加成",1503 },
            {"速度",1003 },
            {"效果抵抗",1504 },
            {"效果命中",1504 },
            {"防御加成",3003 },
            {"生命加成",1503 },
        };

        public static int GetShuxingChengzhang(string v)
        {
            try
            {
                return shuxingchengzhang[v] % 10;
            }
            catch { return 3; }
        }
        public static int Get2jiantaoval(string v)
        {
            try
            {
                return shuxingchengzhang[v] / 100;
            }
            catch { return 15; }
        }


        public static List<string>[] posibleShuxing = new List<string>[6]
        {
            new List<string>() { "攻击"},
            new List<string>() { "速度","攻击加成","防御加成","生命加成",},
            new List<string>() { "防御"},
            new List<string>() { "效果抵抗","效果命中","攻击加成","防御加成","生命加成",},
            new List<string>() { "生命"},
            new List<string>() {  "暴击伤害","暴击", "攻击加成","防御加成","生命加成",}
        };
        public static Dictionary<string, List<double>> ZhushuxingDict = new Dictionary<string, List<double>>()
        {
            { "速度",new List<double>() { 57} },
            { "暴击伤害",new List<double>() { 89} },
            { "攻击",new List<double>() { 486, 395 } },
            { "防御",new List<double>() { 104, 86 } },
            { "生命",new List<double>() { 2052, 1654 } },
        };

    }
}
