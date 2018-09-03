using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuHunYYS
{
    [JsonObject(MemberSerialization.OptIn)]
    public class YuHun: ViewModel
    {
        int index = 0;
        [JsonProperty]
        public int Index { get { return index; } set { index = value; Notify("Index"); } }

        public string Weizhi { get { return AppSettings.indexStringList[index]; } }

        ShuxingWithValue[] shuxings = new ShuxingWithValue[5];

        [JsonProperty]
        public string GUID { get; set; }
        public YuHun()
        {
            if (string.IsNullOrWhiteSpace(GUID))
                GUID = Guid.NewGuid().ToString();
            if (shuxings[0] == null)
                for (int x = 0; x < 5; x++) shuxings[x] = new ShuxingWithValue();
        }

        [JsonProperty]
        public ShuxingWithValue[] Shuxings
        {
            get { return shuxings; }
            set
            {
                shuxings = value;
                Display = "";
            }
        }

        public ShuxingWithValue this[int index]
        {
            get { return shuxings[index]; }
            set
            {
                shuxings[index] = value;
            }
        }

        void updateDisplay()
        {
            ds = "";
            foreach (var x in Shuxings) if (x != null)
                    ds += x.Display + '\n';
            display += TypeName + '\n' + ds;
        }


        [JsonProperty]
        public string TypeName { get; set; }

        YuHunType type;
        public YuHunType Type
        {
            get { return type; }
            set
            {
                type = value;
                TypeName = value.Name;
            }
        }

        bool isusing = true;
        [JsonProperty]
        public bool IsUsing
        {
            get { return isusing; }
            set
            {
                isusing = value;
                Notify("IsUsing");
            }
        }

        string display;
        public string DisplayWithType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(display)||display.Length<10)
                    updateDisplay();
                return display;
            }
            set { display = value; }
        }


        string ds;
        public string Display
        {
            get
            {
                if (string.IsNullOrWhiteSpace(display) || display.Length < 10)
                    updateDisplay();
                return ds;
            }
            set
            { ds = value; }
        }

    }
}
