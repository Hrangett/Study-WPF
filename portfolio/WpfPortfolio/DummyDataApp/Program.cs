using Bogus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace DummyDataApp
{
    public class Program
    {
        public static string MqttBrokerUrl { get; set; }//static메소드에 상ㅇ하니까 static 선언 별 의미 없음
        public static MqttClient Client { get; set; }
        private static Thread MqttThread { get; set; }
        private static Faker<SensorInfo> SensorData { get; set; }
        private static string CurrValue { get; set; }


        static void Main(string[] args)
        {
    /* ================================================================================ */

            InitializeConfig();     //구성초기화
            ConnectMqttBroker();    //브로커 접속
            StaticPublish();        //배포(Publish 발행)

    /* ================================================================================ */


        }

        private static void InitializeConfig()
        {
            //구성 초기화
            MqttBrokerUrl = "210.119.12.71";
            var Rooms = new[] { "DINNING", "LIVING", "BATH", "BED" };    //부엌,거실,욕실,침실
            SensorData = new Faker<SensorInfo>()
                    .RuleFor(o => o.DevId, f => f.PickRandom(Rooms))
                    .RuleFor(r => r.CurrTime, f => f.Date.Past(0))//0을 넣으면 현재시간을 넣을 수 있다
                    .RuleFor(r => r.Temp, f => f.Random.Float(19.0f, 30.9f))
                    .RuleFor(r => r.Humid, f => f.Random.Float(40.0f, 63.9f));


        }

        private static void ConnectMqttBroker()
        {
            
            try
            {
                Client = new MqttClient(MqttBrokerUrl);
                Client.Connect("SMARTHOME01");    //connect 할 client의 이름 적기
                Console.WriteLine("접속 성공");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"접속 불가 :: {ex}");
                Environment.Exit(5);    //Error_ACCESS_DENIEN
            }
        }

        private static void StaticPublish()
        {
            MqttThread = new Thread(() => LoopPublish());
            MqttThread.Start();

            //Thread thread2 = new Thread(() => LoopPublish2());
            //thread2.Start();

            //Thread thread3 = new Thread(() => LoopPublish3());
            //thread3.Start();

        }

        //private static void LoopPublish3()
        //{
        //    while (true)
        //    {
        //        SensorInfo tempValue = SensorData.Generate();

        //        tempValue.DevId = Guid.NewGuid().ToString();    // **newData topic DEVID 변경**

        //        CurrValue = JsonConvert.SerializeObject(tempValue, Formatting.Indented);
        //        Client.Publish("home/device/newdata22/", Encoding.Default.GetBytes(CurrValue));
        //        Console.WriteLine($"Publiched newdata22 : {CurrValue}");
        //        Thread.Sleep(500);
        //    }
        //}

        //LoopPulish와 별개로 동작하는 태스크
        //private static void LoopPublish2()
        //{
        //    while (true)
        //    {
        //        SensorInfo tempValue = SensorData.Generate();
        //        tempValue.DevId = Guid.NewGuid().ToString();    //newData topic DEVID 변경

        //        CurrValue = JsonConvert.SerializeObject(tempValue, Formatting.Indented);
        //        Client.Publish("home/device/newdata/", Encoding.Default.GetBytes(CurrValue));
        //        Console.WriteLine($"Publiched newdata : {CurrValue}");
        //        Thread.Sleep(1500);
        //    }
        //}

        private static void LoopPublish()
        {
            while (true)
            {
                SensorInfo tempValue = SensorData.Generate();
                CurrValue = JsonConvert.SerializeObject(tempValue, Formatting.Indented);
                Client.Publish("home/device/fakedata/", Encoding.Default.GetBytes(CurrValue));
                Console.WriteLine($"Publiched fakedata : {CurrValue}");
                Thread.Sleep(1000);
            }

        }
    }
}
