using Bogus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DummyDataApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int num = 1;
            while (true)
            {
                var Rooms = new[] { "DINING", "LIVING", "BATH", "BED" };    //부엌,거실,욕실,침실
                var sensorDummy = new Faker<SensorInfo>()
                    .RuleFor(o => o.DevId, f => f.PickRandom(Rooms))
                    .RuleFor(r => r.CurrTime, f => f.Date.Past(0))//0을 넣으면 현재시간을 넣을 수 있다
                    .RuleFor(r => r.Temp, f => f.Random.Float(19.0f, 30.9f))
                    .RuleFor(r => r.Humid, f => f.Random.Float(40.0f, 63.9f));

                var currValue = sensorDummy.Generate();


                Console.WriteLine(num);
                Console.WriteLine(JsonConvert.SerializeObject(currValue, Formatting.Indented));
                num++;
                Thread.Sleep(1000);
            }
                
        }
    }
}
