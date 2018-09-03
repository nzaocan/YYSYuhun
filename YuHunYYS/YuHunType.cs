using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YuHunYYS
{
    public class YuHunType
    {
        public string Name { get; set; }
        public byte ColorR { get; set; }
        public byte ColorG { get; set; }
        public byte ColorB { get; set; }
        public void SetColor(string st)
        {
            try
            {
                var color = st.Split(',');
                ColorR = Convert.ToByte(color[0]);
                ColorG = Convert.ToByte(color[1]);
                ColorB = Convert.ToByte(color[2]);

            }
            catch
            {
                ColorB = ColorG = ColorR = 255;
            }
        }
        public Color Color
        {
            get
            {
                if (ColorR + ColorG + ColorB < 20) return Colors.White;
                return Color.FromRgb(ColorR, ColorG, ColorB);
            }
        }

        string sxname = "暴击";
        public string ShuxingName
        {
            get { return sxname; }
            set { sxname = value; }
        }
        double v = 15;
        public double Val { get { return v; } set { v = value; } }
        public string Note { get; set; }
    }



    [JsonObject(MemberSerialization.OptIn)]

    public class ShuxingWithValue
    {
        string name;
        [JsonProperty]
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }
        [JsonProperty]
        public double Val { get; set; }
        public string Display
        {
            get
            {
                try
                {
                    return string.Format("{0} : {1}", Name, Val);
                }
                catch { return ""; }
            }
        }
    }
}
