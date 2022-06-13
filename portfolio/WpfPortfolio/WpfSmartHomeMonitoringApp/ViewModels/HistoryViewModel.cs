using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfSmartHomeMonitoringApp.Helpers;
using WpfSmartHomeMonitoringApp.Models;

namespace WpfSmartHomeMonitoringApp.ViewModels
{
    //HistoryModel
    //    SearchIoTData()
    //    TotalCount
    //    InitEndDate
    //    EndDate
    //    InitStartDate
    //    StartDate
    //    DivisionVal
    //SelectedDivision
    //    Divisions
    public class HistoryViewModel : Screen
    {
        //xaml 초기 설정 :: 생성자에서 초기화가 되어야 그 다음 스탭으로 진행 할 수 있다ㅜㅜ 이름규칙!!!

        private BindableCollection<DivisionModel> divisions;    //BindableCollection : 객체의 속성이 변경될 떄 마다 외부에 알리는 역활을 한다.
        private DivisionModel selectedDivision;
        private string startDate;
        private string initStartDate;
        private string initEndDate;
        private string endDate;
        private int totalCount;
        private PlotModel historyModel; //OxyPlot : 220613, LEU smartHomeModel -> historyModel 변경

        public string StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }

        public string InitStartDate
        {
            get => initStartDate;
            set
            {
                initStartDate = value;
                NotifyOfPropertyChange(() => InitStartDate);
            }
        }
        public string EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }
        public int TotalCount
        {
            get => totalCount;
            set
            {
                totalCount = value;
                NotifyOfPropertyChange(() => TotalCount);
            }
        }
        public PlotModel HistoryModel   //OxyPlot : 220613, LEU smartHomeModel -> historyModel 변경
        {
            get => historyModel;
            set
            {
                historyModel = value;
                NotifyOfPropertyChange(() => HistoryModel);
            }
        }
        public BindableCollection<DivisionModel> Divisions
        {
            get => divisions;
            set
            {
                divisions = value;
                NotifyOfPropertyChange(() => Divisions);
            }
        }
        public DivisionModel SelectedDivision
        {
            get => selectedDivision;
            set
            {
                selectedDivision = value;
                NotifyOfPropertyChange(() => SelectedDivision);
            }
        }

        public string InitEndDate
        {
            get => initEndDate; 
            set
            {
                initEndDate = value;
                NotifyOfPropertyChange(() => InitEndDate);

            }
        }

        public HistoryViewModel()
        {
            //구성 초기화
            Commons.CONNSTRING = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";

            InitControl();

        }

        private void InitControl()
        {
            Divisions = new BindableCollection<DivisionModel>   //콤보박스용 데이터 생성
            {
                new DivisionModel {KeyVal = 0, DivisionVal = "-- Select --"},
                new DivisionModel {KeyVal = 1, DivisionVal = "DINNING"},
                new DivisionModel {KeyVal = 2, DivisionVal = "LIVING"},
                new DivisionModel {KeyVal = 3, DivisionVal = "BED"},
                new DivisionModel {KeyVal = 4, DivisionVal = "BATH"},
            };

            //Select 선택해 초기화
            SelectedDivision = Divisions.Where(v => v.DivisionVal.Contains("Select")).FirstOrDefault();
            InitStartDate = DateTime.Now.ToShortDateString();   //2022-06-10
            InitEndDate = DateTime.Now.AddDays(1).ToShortDateString();  //2022-06-11
        }

        //검색메서드
        public void SearchIoTData()
        {
            if(SelectedDivision.KeyVal == 0)
            {
                MessageBox.Show("검색할 방을 선택하시오");
                return;
            }

            if (DateTime.Parse(StartDate) > DateTime.Parse(EndDate))
            {
                MessageBox.Show("날짜가 올바르지 않습니다 다시 선택");
                return;
            }

            TotalCount = 0;

            using (SqlConnection conn = new SqlConnection(Commons.CONNSTRING))
            {
                string strQuery = @"SELECT Id, CurrTime, Temp, Humid
                                    FROM TblSmartHome
                                    WHERE DevId = @DevId
                                      AND CurrTime BETWEEN @StartDate AND @EndDate
                                    ORDER BY Id ASC ";

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(strQuery, conn);

                    SqlParameter parmDevId = new SqlParameter("@DevId", SelectedDivision.DivisionVal);
                    cmd.Parameters.Add(parmDevId);

                    SqlParameter parmStartDate = new SqlParameter("@StartDate", StartDate);
                    cmd.Parameters.Add(parmStartDate);

                    SqlParameter parmEndDate = new SqlParameter("@EndDate", EndDate);
                    cmd.Parameters.Add(parmEndDate);

                    SqlDataReader reader = cmd.ExecuteReader();

                    var i = 0;

                    //start of shart process 
                    var tmp = new PlotModel
                    {
                        Title = $"{ SelectedDivision.DivisionVal }Histories",
                        Subtitle = "using OxyPlot"
                    };   //임시 플롯 모델

                    //범례 : OxyPlot.Wpf >> LEgendsDemo 참조
                    var l = new Legend
                    {
                        LegendBorder = OxyColors.Black,
                        LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                        LegendPosition = LegendPosition.RightTop,
                        LegendPlacement = LegendPlacement.Inside

                    };

                    var seriesTemp = new LineSeries
                    {
                        Color = OxyColor.FromRgb(205, 100, 0),
                        Title = "Temperature",
                        MarkerSize = 4,
                        MarkerType = MarkerType.Circle
                    };  //온도값을 라인차트로 담을 객체

                    LineSeries seriesHumid = new LineSeries
                    {
                        Color = OxyColor.FromRgb(150,150,255),
                        Title = "Humidity",
                        MarkerType = MarkerType.Triangle
                    };


                    while (reader.Read())
                    {

                        //var temp = reader["Temp"];
                        //Temp, Humid 차트 데이터 생성
                        
                        seriesTemp.Points.Add(new DataPoint(i, Convert.ToDouble(reader["Temp"])));    //DataPoint(x,y)
                        seriesHumid.Points.Add(new DataPoint(i, Convert.ToDouble(reader["Humid"])));    //DataPoint(x,y)

                        i++;
                    }

                    TotalCount = i; //검색한 데이터의 총 갯수

                    tmp.Series.Add(seriesTemp);
                    tmp.Series.Add(seriesHumid);
                    HistoryModel = tmp;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error {ex.Message}");
                    return;

                }
            }
            
        }
    }
}
