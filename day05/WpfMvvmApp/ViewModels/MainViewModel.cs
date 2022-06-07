using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfMvvmApp.Models;


namespace WpfMvvmApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        //View에서 사용 하기위해 정의
        private string inFirstName;
        private string inLastName;
        private string inEmail;
        private DateTime inDate;

        private string outFirstName;
        private string outLastName;
        private string outEmail;
        private string outDate;

        private string outAdult;
        private string outBirthday;



        //=======실제 사용되는 속성=======

        public MainViewModel()
        {
            this.inDate = DateTime.Parse("1990-01-01");
        }

        public string InFirstName
        {
            get
            {
                return inFirstName;
            }
            set
            {
                inFirstName = value;
                RaisePropertyChanged("InFirstName...");    //값이 바뀜을 알려줌
            }
        }

        public string InLastName
        {
            get
            {
                return inLastName;
            }
            set
            {
                inLastName = value;
                RaisePropertyChanged("InLastName...");    //값이 바뀜을 알려줌
            }
        }

        public string InEmail
        {
            get
            {
                return inEmail;
            }
            set
            {
                inEmail = value;
                RaisePropertyChanged("******InEmail*****");    //값이 바뀜을 알려줌
            }
        }

        public DateTime InDate
        {
            get
            {
                return inDate;
            }
            set
            {
                inDate = value;
                RaisePropertyChanged("InDate");    //값이 바뀜을 알려줌
            }
        }

        public string OutFirstName
        {
            get
            {
                return outFirstName;
            }
            set
            {
                outFirstName = value;
                RaisePropertyChanged("OutFirstName");    //값이 바뀜을 알려줌
            }
        }

        public string OutLastName
        {
            get
            {
                return outLastName;
            }
            set
            {
                outLastName = value;
                RaisePropertyChanged("OutLastName");    //값이 바뀜을 알려줌
            }
        }

        public string OutEmail
        {
            get
            {
                return outEmail;
            }
            set
            {
                outEmail = value;
                RaisePropertyChanged("OutEmail");    //값이 바뀜을 알려줌
            }
        }

        public string OutDate
        {
            get
            {
                return outDate;
            }
            set
            {
                outDate = value;
                RaisePropertyChanged("OutDate");    //값이 바뀜을 알려줌
            }
        }

        public string OutAdult
        {
            get
            {
                return outAdult;
            }
            set
            {
                outAdult = value;
                RaisePropertyChanged("OutAdult");    //값이 바뀜을 알려줌
            }
        }

        public string OutBirthday
        {
            get
            {
                return outBirthday;
            }
            set
            {
                outBirthday = value;
                RaisePropertyChanged("OutBirthday");    //값이 바뀜을 알려줌
            }
        }


        //전부 값이 적용되어 버튼을 활성화 하기 위한 명령어
        private ICommand proceedcommand;


        public ICommand ProceedCommand
        {
            get{
                return proceedcommand ?? (new RelayCommand<object>(
                        o => Proceed(), o => !string.IsNullOrEmpty(inFirstName) &&
                           !string.IsNullOrEmpty(inLastName) &&
                           !string.IsNullOrEmpty(inEmail) &&
                           !string.IsNullOrEmpty(inDate.ToString())
                    ));


            }
        }

        //버튼 클릭시 일어나는 실제 명령의 실체
        private async void Proceed()
        {
            try
            {
                Person person = new Person(inFirstName, inLastName, inEmail, inDate);

                await Task.Run(() => OutFirstName = person.FirstName);
                await Task.Run(() => OutLastName = person.LastName);
                await Task.Run(() => OutEmail = person.Email);
                await Task.Run(() => OutDate = person.Date.ToString("yyyy-MM-dd"));
                await Task.Run(() => OutBirthday = $"{person.IsBirthday}");
                await Task.Run(() => OutAdult = $"{person.IsAdult}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error : {ex}");
                
            }

        }
    }
}
