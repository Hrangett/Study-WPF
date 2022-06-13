using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    class CustomPopupViewModel : Conductor<object>
    {
        private string brokerIp;
        private string topic;

        public string BrokerIp
        {
            get => brokerIp; 
            set
            {
                brokerIp = value;
                NotifyOfPropertyChange(() => BrokerIp);
            }
        }
        public string Topic
        {
            get => topic; 
            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }

        public CustomPopupViewModel(string title)
        {
            this.DisplayName = title;

            BrokerIp = "127.0.0.1";
            Topic = "home/device/#";
        }

        public void AcceptClose()
        {
            Commons.BROKERHOST = BrokerIp;
            Commons.PUB_TOPIC = Topic;

            TryCloseAsync(true);

        }
    }
}
