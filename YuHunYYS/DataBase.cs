using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YuHunYYS
{
    public class DataBase:ViewModel
    {
        public ObservableCollection<YuHun> YuhunList { get; set; }

        public DataBase()
        {
        }

        public void Init()
        {
            if (YuhunList == null)
                YuhunList = new ObservableCollection<YuHun>();
            for (int i = 0; i < 6; i++)
                yuhunInIndex[i] = new List<YuHun>();
            foreach (var x in YuhunList)
            {
                x.Type = App.Settings.YuhunTypeList.First(i => i.Name == x.TypeName);
            }
        }

        void resetYuhunInIndex(List<YuHun> yh)
        {
            foreach (var x in yuhunInIndex)
                x.Clear();

            foreach (var x in yh)
            {
                if(x.IsUsing)
                    yuhunInIndex[x.Index].Add(x);
            }
        }


        public void RemoveYuhun(List<YuHun> list)
        {
            foreach (var x in list)
            {
                if (x != null && YuhunList.Contains(x))
                    YuhunList.Remove(x);
            }
        }
        public void RemoveYuhun(YuHun yh)
        {
            try
            {
                YuhunList.Remove(yh);
            }
            catch { }
        }

        public void AddYuhun(YuHun y)
        {
            YuhunList.Add(y);
        }
        public void AddYuhun()
        {
            YuhunList.Add(App.Settings.TempYuhun);
        }

        public List<YuHun> GetYuhunListNotInYuhunFangan()
        {
            var l= YuhunList.ToList();
            var list = App.Settings.YuhunFanganList;
            if (list == null) return l;

            foreach (var fangan in list) if(fangan.IsUsing)
            {
                l.RemoveAll(x => fangan.Yuhuns.Contains(x));
            }

            return l;
        }


        void calYuhun(Dictionary<string, double> d,YuHun yh,int nSameType,char c,Shishen ss)
        {
            int param = 1;
            if (c == '-') param = -1;

            if (nSameType == 1 || nSameType == 5)
                d[yh.Type.ShuxingName] += yh.Type.Val * param;

            foreach (var x in yh.Shuxings)
            {
                switch (x.Name)
                {
                    case "攻击":
                        d["攻击加成"] += 100*(x.Val / ss.Gongji) * param;
                        break;
                    case "防御":
                        d["防御加成"] += 100 * (x.Val / ss.Fangyu) * param;
                        break;
                    case "生命":
                        d["生命加成"] += 100 * (x.Val / ss.Shengming) * param;
                        break;
                    default:
                        d[x.Name] += x.Val * param;
                        break;
                }
            }

        }

        void calYuhun(Dictionary<string, double> d, YuHun yh, int nSameType)
        {
            calYuhun(d, yh, nSameType, '+', tempShishen);
        }

        void optimizeAns(Dictionary<string, double> valDict)
        {
            var td = new Dictionary<string, double>();
            foreach (var x in valDict) td[x.Key] = x.Value;

            foreach (var x in tempLimits)
            {
                if (td.ContainsKey(x.Name))
                {
                    var item = td[x.Name];
                    if (item < x.left || item > x.right)
                    {
                        return;
                    }
                }
            }

            var t = ans;
            if (ans == null) ans = td;
            else if (goalType == GoalType.single)
            {
                if (ans[goalParam1] < td[goalParam1])
                    ans = td;
            }
            else if (goalType == GoalType.plus)
            {
                if (ans[goalParam1] + ans[goalParam2] < td[goalParam1] + td[goalParam2])
                    ans = td;
            }
            else
            {
                if (ans[goalParam1] * ans[goalParam2] < td[goalParam1] * td[goalParam2])
                    ans = td;
            }
            //if value changed
            if (ans != t)
            {
                for (int i = 0; i < 6; i++)
                    ansYuhun[temYuhun[i].Index] = temYuhun[i];


            }
        }

        public double maxTheoryVal(int depth, string sxname)
        {
            double maxVal = 0;
            var v1 = 6 * AppSettings.GetShuxingChengzhang(sxname);
            int sy = (6 - depth);
            maxVal += v1 * sy;
            var v2 = AppSettings.Get2jiantaoval(sxname);

            var dict = new Dictionary<YuHunType, int>();
            for (var i = 0; i < depth; i++)
            {
                if (dict.ContainsKey(temYuhun[i].Type))
                {
                    var t = dict[temYuhun[i].Type];
                    if (t == 4) t = 0;
                    dict[temYuhun[i].Type] = t + 1;
                }
                else
                    dict[temYuhun[i].Type] = 1;
            }

            foreach (var x in dict)
            {
                if (x.Value == 1)
                {
                    if (x.Key.ShuxingName == sxname && sy > 0)
                    {
                        sy--;
                        maxVal += v2;
                    }
                }
                else if (x.Value == 3 && !isSanjian)
                {
                    sy--;
                }
            }
            if (sy > 0) maxVal += (sy / 2) * v2;

            if (depth < 3)
            {
                if (sxname == "速度")
                    maxVal += 57;
            }
            if (depth == 1)
            {
                if (sxname == "效果抵抗" || sxname == "效果命中")
                    maxVal += 55;
            }
            return maxVal;
        }


        bool check(Dictionary<string, double> valDict,int depth)
        {
            if (temYuhun[0].TypeName == "网切" && valDict["暴击"] > 100 && valDict["暴击伤害"]>270 && temYuhun[4].TypeName == "网切" && depth==6)
            {
            }
            foreach (var x in valDict)
                if (x.Value > topVal[x.Key])
                    return false;


            foreach (var x in valDict)
                if (x.Value + maxTheoryVal(depth, x.Key)+0.005 < bottomVal[x.Key])
                    return false;

            var d = new Dictionary<string, int>();
            for (int i = 0; i < depth; i++)
            {
                var nm = temYuhun[i].TypeName;
                if (!d.ContainsKey(nm)) d[nm] = 1;
                else d[nm]++;
            }

            if (!isSanjian)
            {
                int n = 0;
                if(d.ContainsKey(tempsijiantao.Name))
                    n = d[tempsijiantao.Name];
                if (d.Count > 3 || n < depth - 2)
                {
                    return false;
                }
            }
            return true;
        }

        Shishen tempShishen { get; set; }
        List<Limit> tempLimits { get; set; }


        int[] searchOrder = new int[6] { 5, 3, 1, 0, 2, 4 };
        void search(int depth, Dictionary<string, double> valDict)
        {
            if (depth == 1)
            {
                ncurrent+=2;
                nProcess = (int)((double)ncurrent / nProcessTotal * 100);
            }
            if (depth != 0)
            {
                var b= check(valDict,depth);
                if (!b) return;
            }
            if (depth == 6)
            {
                optimizeAns(valDict);
                return;
            }
            foreach (var x in yuhunInIndex[searchOrder[depth]])
            {

                var n = 0;
                for (var i = 0; i < depth; i++)
                    if (temYuhun[i].TypeName == x.TypeName) n++;
                calYuhun(valDict, x,n);
                temYuhun[depth] = x;
                search(depth + 1, valDict);
                calYuhun(valDict, x,n, '-', tempShishen);
            }
        }

        List<YuHun>[] yuhunInIndex = new List<YuHun>[6];
        static char[] splitChar = new char[] { '+', '*' };
        string goalParam1 = "", goalParam2 = "";
        int nProcessTotal = 0, ncurrent = 0;
        GoalType goalType = 0;
        Dictionary<string, double> ans;
        YuHunType tempsijiantao;
        bool isSanjian = false;
        Dictionary<string, double> topVal = new Dictionary<string, double>();
        Dictionary<string, double> bottomVal = new Dictionary<string, double>();
        YuHun[] ansYuhun = new YuHun[6];
        YuHun[] temYuhun = new YuHun[6];
        HashSet<string> NotFreeShuxingList { get; set; }
        HashSet<string> MainShuxingList { get; set; }

        public const int MaxInt = 1000000;

        string errorInputCorrect(string st)
        {
            if (st == "攻击") return "攻击加成";
            if (st == "防御") return "防御加成";
            if (st == "生命") return "生命加成";
            if (st == "暴伤") return "暴击伤害";
            if (st == "爆伤") return "暴击伤害";
            if (st == "抵抗") return "效果抵抗";
            if (st == "命中") return "效果命中";
            return st;
        }

        int nprocess = 0;
        public int nProcess
        {
            get { return nprocess; }
            set
            {
                if (value > 100) value = 100;
                if (nprocess == value) return;
                nprocess = value;
                Notify("nProcess");
            }
        }

        public YuhunFangan GetYuhunFangan(List<Limit> limits, Shishen ss, string goal,YuHunType sijiantao)
        {
            ncurrent=nProcessTotal = nProcess = 0;
            var g = goal.Split(splitChar).ToList();
            g.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            if (g.Count > 2)
            {
                MessageBox.Show("目标格式错误");
                return null;
            }
            var allshuxing = App.Settings.ShuxingList;
            for (int i = 0; i < g.Count; i++)
                g[i] = errorInputCorrect(g[i]);

            limits.RemoveAll(x => string.IsNullOrWhiteSpace(x.Name));
            foreach (var x in limits)
            {
                x.Name = errorInputCorrect(x.Name);
            }

            foreach (var x in g)
                if (!allshuxing.Contains(x))
                {
                    MessageBox.Show("目标格式错误");
                    return null;
                }

            bool isShuchu = false;
            bool isBaoshang = false;
            bool isSudu = false;
            bool isBaoji = false;
            bool isShengming = false;
            bool isFangyu = false;
            bool isDikang = false;
            bool isMingzhong = false;

            if (goal.Contains("攻击")) isShuchu = true;
            if (goal.Contains("伤")) isBaoshang = true;
            if (goal.Contains("暴击")) isBaoji = true;
            if (goal.Contains("防御")) isFangyu = true;
            if (goal.Contains("生命")) isShengming = true;
            if (goal.Contains("速度")) isSudu = true;
            if (goal.Contains("命中")) isMingzhong = true;
            if (goal.Contains("抵抗")) isDikang = true;

            foreach (var x in limits)
            {
                if (x.Name == "速度" && x.left-ss.Sudu > 56)
                    isSudu = true;
                if (x.Name == "暴击伤害" && x.left -ss.Baoshang >= 89)
                    isBaoshang = true;
                if (x.Name == "暴击" && x.left > 84)
                    isBaoji = true;
                if (x.Name == "攻击加成" && x.left > 170)
                    isShuchu = true;
                if (x.Name == "防御加成" && x.left > 185)
                    isFangyu = true;
                if (x.Name == "生命加成" && x.left > 185)
                    isShengming = true;
                if (x.Name == "效果抵抗" && x.left-ss.Dikang > 80)
                    isDikang = true;
                if (x.Name == "效果命中" && x.left-ss.Mingzhong > 80)
                    isMingzhong = true;
            }


            //初始化


            for (int i = 0; i < 8; i++)
            {
                topVal[AppSettings.shuxingList[i]] = MaxInt;
                bottomVal[AppSettings.shuxingList[i]] = 0;
            }
            topVal["暴击"] = 114;
            foreach (var x in limits)
            {
                if (topVal.ContainsKey(x.Name))
                    topVal[x.Name] = x.right;
                if (bottomVal.ContainsKey(x.Name))
                    bottomVal[x.Name] = x.left;
            }


            goalParam1 = goalParam2 = null;
            goalType = 0;
            ans = null;
            ansYuhun = new YuHun[6];
            tempsijiantao = sijiantao;
            if (tempsijiantao != null)
                isSanjian = string.IsNullOrWhiteSpace(tempsijiantao.Name);
            else
                isSanjian = true;
            tempShishen = ss;
            tempLimits = limits;
            goalParam1 = g[0];
            if (g.Count > 1)
                goalParam2 = g[1];
            if (goal.Contains('+')&&g.Count==2)
                goalType = GoalType.plus;
            else if (goal.Contains('*') && g.Count == 2)
                goalType = GoalType.mul;

            var srcList = GetYuhunListNotInYuhunFangan();

            //第一步，主属性
            if (!isBaoshang)
                srcList.RemoveAll(x => x.Shuxings[0].Name == "暴击伤害");
            if (!isSudu)
                srcList.RemoveAll(x => x.Shuxings[0].Name == "速度");
            if (!isDikang)
                srcList.RemoveAll(x => x.Shuxings[0].Name == "效果抵抗");
            if (!isMingzhong)
                srcList.RemoveAll(x => x.Shuxings[0].Name == "效果命中");
            if (!isBaoji)
                srcList.RemoveAll(x => x.Shuxings[0].Name == "暴击");

            if (isFangyu || isFangyu || isShuchu)
            {
                if(!isShuchu)
                    srcList.RemoveAll(x => x.Shuxings[0].Name == "攻击加成");
                if (!isShengming)
                    srcList.RemoveAll(x => x.Shuxings[0].Name == "生命加成");
                if (!isFangyu)
                    srcList.RemoveAll(x => x.Shuxings[0].Name == "防御加成");
            }


            //第二步，删除所有属性值教弱且不能包含不自由属性的御魂

            {
                var notfreeShuxingList = new HashSet<string>();
                var mainShuxingList = new HashSet<string>();
                foreach (var x in topVal)
                    if (x.Value < 200)
                    {
                        notfreeShuxingList.Add(x.Key);
                    }

                foreach (var x in g)
                {
                    mainShuxingList.Add(x);
                }
                foreach (var x in limits)
                {
                    if (x.left > 0 && !notfreeShuxingList.Contains(x.Name))
                    {
                        mainShuxingList.Add(x.Name);
                    }
                }
                NotFreeShuxingList = notfreeShuxingList;
                MainShuxingList = mainShuxingList;


                foreach (var x in srcList.ToList())
                {
                    var needRemoveList = new List<YuHun>();

                    foreach (var y in srcList)
                    {

                        //如果类型不同，主属性不同，直接跳过
                        if (y == x || y.Type != x.Type || x[0].Name != y[0].Name)
                            continue;

                        //两件套属性相同，但是是四件套，跳过

                        //if ( y.TypeName!=x.TypeName && (y.TypeName == sijiantao.Name || x.TypeName==sijiantao.Name))
                        //    continue;

                        var containsNotFree = false;
                        for (int i = 1; i < 5; i++)
                            if (notfreeShuxingList.Contains(y[i].Name))
                            {
                                containsNotFree = true;
                                break;
                            }
                        //包含不自由属性，直接跳过
                        if (containsNotFree) continue;

                        var isyx = false;
                        for (int i = 1; i < 5; i++)
                            if (mainShuxingList.Contains(y[i].Name))
                            {
                                double vy = y[i].Val;
                                double vx = 0;
                                for (int j = 1; j < 5; j++)
                                    if (x[j].Name == y[i].Name)
                                    {
                                        vx = x[j].Val;
                                        break;
                                    }
                                if (vy > vx)
                                {
                                    isyx = true;
                                    break;
                                }
                            }

                        //如果y有属性大于x，跳过
                        if (isyx) continue;

                        needRemoveList.Add(y);
                    }

                    foreach (var item in needRemoveList)
                        srcList.Remove(item);
                }

                resetYuhunInIndex(srcList);

            }
            

            Dictionary<string, double> valDict = new Dictionary<string, double>();
            valDict["暴击"] = ss.Baoji;
            valDict["暴击伤害"] = ss.Baoshang;
            valDict["效果命中"] = ss.Mingzhong;
            valDict["效果抵抗"] = ss.Dikang;
            valDict["速度"] = ss.Sudu;
            valDict["防御加成"] = 100;
            valDict["生命加成"] = 100;
            valDict["攻击加成"] = 100;

            nProcessTotal = yuhunInIndex[0].Count;
            search(0, valDict);

            if (ansYuhun[0] == null) return null;

            var result = new YuhunFangan(goal, limits, ansYuhun);


            var fast = "";
            foreach (var x in App.Settings.YuhunFanganList)
                if (x.IsUsing) fast += string.Format("[{0}] ", x.Name);
                
            var st = "";
            st += string.Format("式神：{0}\t四件套：{1}\t以下方案中的御魂没有参与计算：{2}\n\n", ss.Name, isSanjian?"散件":sijiantao.Name, fast);
            st += string.Format("攻击：{0:0.0}\t暴击伤害：{1:0.0}\n", ans["攻击加成"]*ss.Gongji / 100, ans["暴击伤害"]);
            st += string.Format("暴击：{0:0.0}\t速度：{1:0.0}\n", ans["暴击"], ans["速度"]);
            st += string.Format("生命：{0:0.0}\t抵抗：{1:0.0}\n", ans["生命加成"] * ss.Shengming / 100, ans["效果抵抗"]);
            st += string.Format("防御：{0:0.0}\t命中：{1:0.0}\n\n", ans["防御加成"] * ss.Fangyu / 100, ans["效果命中"]);

            int ii = 0;
            foreach (var x in ans)
            {
                st += string.Format("[{0}:{1:0.00}] ", x.Key, x.Value);
                ++ii;
                if (ii == 4)
                    st += '\n';
            }

            result.ResultText = st;

            return result;
        }
    }
}
