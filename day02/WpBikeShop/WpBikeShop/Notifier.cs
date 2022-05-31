using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpBikeShop
{
    public class Notifier : INotifyPropertyChanged  //property의 변화를 알아채겠다
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
