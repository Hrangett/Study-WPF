using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpBikeShop
{
    /// <summary>
    /// Bindings.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Bindings : Page
    {
        public Bindings()
        {
            InitializeComponent();  //화면기본구성 :: Don't Touch

            Car c = new Car
            {
                Speed = 100,
                Color = Colors.Crimson,
                Driver = new Human
                {
                    Firstname = "Nick",
                    HasDrivingLicense = true


                }
            };
            //txtSpeed.Text = c.Speed.ToString();   //고전적인 윈폼 방식

            //txtSpeed.DataContext = c;   //필수 :: 이 데이터를 xaml으로 보내야 함
            //txtColor.DataContext = c;
            //txtFirstName.DataContext = c;

            //stxPanel.DataContext = c;   //컨텐츠를 감싸고 있는 패널에 객체를 대입하면 그 속에 속해있는 패널도 그 값을 사용 할 수 있다
            this.DataContext = c;   //개꿀?
            
            var carList = new List<Car>();
            for (int i = 0; i < 10; i++)
            {
                carList.Add(new Car
                {
                    Speed = i * 10,
                    Color = Colors.Purple
                });
                
            }

            lbxCars.DataContext = carList;

        }
    }
}
