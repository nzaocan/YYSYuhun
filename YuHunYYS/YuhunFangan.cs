using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuHunYYS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class YuhunFangan
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Goal { get; set; }
        [JsonProperty]
        public List<Limit> Limits { get; set; }

        public string Sijiantao { get; set; }
        bool isusing = true;

        [JsonProperty]
        public bool IsUsing
        {
            get
            {
                return isusing;
            }
            set
            {
                isusing = value;
            }
        }
        [JsonProperty]
        public HashSet<string> GuidSet { get; set; }
        public YuHun[] Yuhuns
        {
            get; set;
        }

        [JsonProperty]
        public string ResultText { get; set; }
        public List<YuHun> YuhunList
        {
            get { return Yuhuns.ToList(); }
        }

        public YuhunFangan(string goal, List<Limit> lmits,YuHun[] yuhuns)
        {
            Goal = goal;
            Limits = lmits;
            GuidSet = new HashSet<string>();
            foreach (var x in yuhuns)
                GuidSet.Add(x.GUID);
            Init();
        }

        public YuhunFangan()
        {

        }

        public string YuhunDisplay
        {
            get;set;
        }

        public string LimitDisplay
        {
            get;
            set;
        }
        public void Init()
        {
            if (Yuhuns != null) return;
            try
            {
                Yuhuns = new YuHun[6];
                YuhunDisplay = "";
                int i = 0;

                var set = new Dictionary<string, int>();

                Sijiantao = "散件";
                foreach (var x in GuidSet)
                {
                    var yh = App.db.YuhunList.First(y => y.GUID == x);
                    Yuhuns[i++] = yh;
                    if (set.ContainsKey(yh.TypeName))
                        set[yh.TypeName]++;
                    else
                        set[yh.TypeName] = 1;

                    if (set[yh.TypeName] == 4)
                        Sijiantao = yh.TypeName;

                    YuhunDisplay += yh.Display + "\n";
                }

                LimitDisplay = "";
                foreach (var x in Limits)
                    LimitDisplay += x.ToString();
            }
            catch
            { }
        }

    }

    public enum GoalType
    {
        single,
        plus,
        mul,
    }
}
