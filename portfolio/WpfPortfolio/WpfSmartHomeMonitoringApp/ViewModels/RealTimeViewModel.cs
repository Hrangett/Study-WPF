﻿using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    class RealTimeViewModel : Screen
    {
        private string livingTempVal;
        private string bedTempVal;
        private string dinningTempVal;
        private string bathTempVal;

        private double livingHumidVal;
        private double dinningHumidVal;
        private double bedHumidVal;
        private double bathHumidVal;
        public RealTimeViewModel()
        {
            Commons.BROKERHOST = "127.0.0.1";   //MQTT Broker IP 설정
            Commons.PUB_TOPIC = "home/device/#";
            
            LivingTempVal = DinningTempVal = BedTempVal = BathTempVal = string.Empty;
            LivingHumidVal = DinningHumidVal = BedHumidVal = BathHumidVal = 0;

            if(Commons.MQTT_CLIENT !=null && Commons.MQTT_CLIENT.IsConnected)
            {
                //접속이 되어있을 경우
                Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
            }
            else
            {
                //접속이 안되어있을 경우 => MQTT Broker에 접속하는 내용
                Commons.MQTT_CLIENT = new MqttClient(Commons.BROKERHOST);
                Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                Commons.MQTT_CLIENT.Connect("Monitor");

                Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

                Commons.IS_CONNECT = true;
            }
        }

        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

            switch(currDatas["DevId"].ToString())
            {
                case "LIVING":
                    LivingTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
                    LivingHumidVal = double.Parse(currDatas["Temp"]);

                    break;
                case "DINNING":
                    DinningTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
                    DinningHumidVal = double.Parse(currDatas["Temp"]);
                    break;
                case "BED":
                    BedTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
                    BedHumidVal = double.Parse(currDatas["Temp"]);
                    break;
                case "BATH":
                    BathTempVal = double.Parse(currDatas["Temp"]).ToString("0.#");
                    BathHumidVal = double.Parse(currDatas["Temp"]);
                    break;

                default:
                    break;



            }
        }

        public string LivingTempVal
        {
            get => livingTempVal; 
            set
            {
                livingTempVal = value;
                NotifyOfPropertyChange(() => LivingTempVal);

            }
        }
        public string DinningTempVal
        {
            get { return dinningTempVal; }
            set
            {
                dinningTempVal = value;
                NotifyOfPropertyChange(() => DinningTempVal);
            }
        }
        public string BedTempVal
        {
            get => bedTempVal; 
            set
            {
                bedTempVal = value;
                NotifyOfPropertyChange(() => BedTempVal);
            }
        }
        public string BathTempVal
        {
            get => bathTempVal; 
            set
            {
                bathTempVal = value;
                NotifyOfPropertyChange(() => BathTempVal);
            }
        }

        public double LivingHumidVal
        {
            get => livingHumidVal; 
            set
            {
                livingHumidVal = value;
                NotifyOfPropertyChange(() => LivingHumidVal);
            }
        }
        public double DinningHumidVal
        {
            get => dinningHumidVal; 
            set
            {
                dinningHumidVal = value;
                NotifyOfPropertyChange(() => DinningHumidVal);
            }
        }
        public double BedHumidVal
        {
            get => bedHumidVal; 
            set
            {
                bedHumidVal = value;
                NotifyOfPropertyChange(() => BedHumidVal);
            }
        }
        public double BathHumidVal
        {
            get => bathHumidVal; 
            set
            {
                bathHumidVal = value;
                NotifyOfPropertyChange(() => BathHumidVal);
            }
        }
    }
}
