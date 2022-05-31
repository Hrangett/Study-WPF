using System.Windows.Media;

namespace WpBikeShop
{
    public class Car
    {
        public double Speed { get; set; }

        public Color Color { get; set; }

        public Human Driver { get; set; }

        public Car()
        {

        }

        

    }

    public class Human
    {

        public string Firstname { get; set; }

        public bool HasDrivingLicense { get; set; }


    }
}