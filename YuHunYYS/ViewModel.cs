using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuHunYYS
{
    public class ViewModel : INotifyPropertyChanged
    {
        string msg { get; set; }
        public string Msg
        {
            get { return msg; }
            set
            {
                msg = value;
                Notify("Msg");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(string PropName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropName));
            }
        }
    }

}
