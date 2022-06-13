using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WpfSmartHomeMonitoringApp.Helpers;
using WpfSmartHomeMonitoringApp.Models;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    public class DataBaseViewModel : Screen
    {
        private string brokerUrl;
        private string topic;
        private string connString;
        private string dbLog;
        private bool isConnected;

        public string BrokerUrl
        {
            get { return brokerUrl; }
            set
            {
                brokerUrl = value;
                NotifyOfPropertyChange(() => BrokerUrl);

            }
        }

        
        public string Topic
        {
            get { return topic; }
            set
            {
                topic = value;
                NotifyOfPropertyChange(() => Topic);
            }
        }
        

       
        public string ConnString
        {
            get => connString; 
            
            set
            {
                connString = value;
                NotifyOfPropertyChange(() => ConnString);
            }
        }

        
        public string DbLog
        {
            get => dbLog; 
            set
            {
                dbLog = value;
                NotifyOfPropertyChange(() => DbLog);
            }
        }

        public bool IsConnected
        {
            get => isConnected; 
            set
            {
                isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
            }
        }

        public DataBaseViewModel()
        {
            BrokerUrl = Commons.BROKERHOST = "127.0.0.1";   //MQTT Broker IP 설정
            Topic = Commons.PUB_TOPIC = "home/device/#";

            //Topic = Commons.PUB_TOPIC = "home/+/#"; :
            //Single Level wildcard : +
            //Multi Level WildCard : #
            
            ConnString = Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";

            if(Commons.IS_CONNECT)
            {
                IsConnected = true;
                ConnectDb();
            }

        }


        /// <summary>
        /// DB연결 + MQTT 접속
        /// </summary>
        public void ConnectDb()
        { 
            if(IsConnected)
            {
                Commons.MQTT_CLIENT = new MqttClient(BrokerUrl);

                try
                {
                    if(Commons.MQTT_CLIENT.IsConnected != true)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived += MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Connect("MONITOR");
                        Commons.MQTT_CLIENT.Subscribe(new string[] { Commons.PUB_TOPIC },
                            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                        
                        UpdateText(">>> MQTT Broker Connected");
                        IsConnected = Commons.IS_CONNECT = true;
                    }
                }
                catch (Exception ex)
                {
                    //Pass
                    
                }

            }
            else
            {
                try
                {
                    if(Commons.MQTT_CLIENT.IsConnected)
                    {
                        Commons.MQTT_CLIENT.MqttMsgPublishReceived -= MQTT_CLIENT_MqttMsgPublishReceived;
                        Commons.MQTT_CLIENT.Disconnect();
                        UpdateText(">>> MQTT Broker Disconnected");
                        IsConnected = Commons.IS_CONNECT = false;
                    }
                }
                catch (Exception)
                {

                    //throw;
                }
            }

        }

        private void UpdateText(string message)
        {
            DbLog += $"{message}\n";
        }

        /// <summary>
        /// Subscribe한 메세지 처리 이벤트 핸들러
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MQTT_CLIENT_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Message);   //bite형식으로 날라온 글짜 다시 string 형으로 바꿔
            UpdateText(message);    //센서데이터 출력
            SetDataBase(message,e.Topic);   //DB에 저장

        }

        private void SetDataBase(string message,string topic)
        {
            //json -> dictionary
            var currDatas = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

            var smartHomeModel = new SmartHomeModel();
            smartHomeModel.DevId = currDatas["DevId"];
            smartHomeModel.CurrTime = DateTime.Parse(currDatas["CurrTime"]);
            smartHomeModel.Temp = double.Parse(currDatas["Temp"]);
            smartHomeModel.Humid = double.Parse(currDatas["Humid"]);

            Debug.WriteLine(currDatas);

            using (SqlConnection conn = new SqlConnection(Commons.CONNSTRING))
            {
                conn.Open();
                //verbatim string
                string strInQuery = @"INSERT INTO TblSmartHome
                                                    (DevId
                                                    ,CurrTime
                                                    ,Temp
                                                    ,Humid)
                                                VALUES
                                                    (@DevId
                                                    ,@CurrTime
                                                    ,@Temp
                                                    ,@Humid)";

                try
                {
                    SqlCommand cmd = new SqlCommand(strInQuery, conn);

                    SqlParameter parmDevId = new SqlParameter("@DevId", smartHomeModel.DevId);
                    cmd.Parameters.Add(parmDevId);

                    SqlParameter parCurrTime = new SqlParameter("@CurrTime", smartHomeModel.CurrTime);    //날짜형으로 변환 필요
                    cmd.Parameters.Add(parCurrTime);

                    SqlParameter parTemp = new SqlParameter("@Temp", smartHomeModel.Temp);
                    cmd.Parameters.Add(parTemp);

                    SqlParameter parHumid = new SqlParameter("@Humid", smartHomeModel.Humid);
                    cmd.Parameters.Add(parHumid);

                    if(cmd.ExecuteNonQuery() == 1)  //영향을 받은 행 수 반환
                    {
                        UpdateText(">>> DB Inserted");
                    }
                    else
                    {
                        UpdateText(">>> !!!!!!! DB Failed !!!!!!");
                    }


                }
                catch (Exception ex)
                {
                    UpdateText($">>> DB Error :: {ex.Message}");
                    
                }

            }//using(){} 은 conn.Close(); 필요 없음


        }
    }
}
