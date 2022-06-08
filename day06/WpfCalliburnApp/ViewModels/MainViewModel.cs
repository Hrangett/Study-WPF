using Caliburn.Micro;
using System.Collections.Generic;
using System.Data.SqlClient;
using WpfCalliburnApp.Models;

namespace WpfCalliburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        //private EmployeesModel employee;   //비쥬얼 스튜디오가 제공하는 이름 쓰길 바람]

        private BindableCollection<EmployeesModel> listEmployees;
        public BindableCollection<EmployeesModel> ListElmployees 
        { 
            get { return listEmployees; }
            set
            {
                listEmployees = value;
                NotifyOfPropertyChange(() => ListElmployees);
            }
        }
        string connString = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";
        public MainViewModel()
        {
            GetEmployees();
        }

        public void GetEmployees()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string strQuery = "SELECT * FROM TblEmployees";
                SqlCommand cmd = new SqlCommand(strQuery, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                ListElmployees = new BindableCollection<EmployeesModel>();

                while (reader.Read())
                {
                    var temp = new EmployeesModel
                    {
                        Id = (int)reader["Id"],
                        EmpName = reader["EmpName"].ToString(),
                        Salary = (decimal)reader["Salary"],
                        DeptName = reader["DeptName"].ToString(),
                        Destination = reader["Destination"].ToString()
                    };
                    ListElmployees.Add(temp);
                }
            }
        }

        int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyOfPropertyChange(() => id);
                NotifyOfPropertyChange(() => CanDelEmployee);
            }
        }

        string empName;
        public string EmpName
        {
            get { return empName; }
            set
            {
                empName = value;
                NotifyOfPropertyChange(() => empName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        decimal salary;
        public decimal Salary
        {
            get { return salary; }
            set
            {
                salary = value;
                NotifyOfPropertyChange(() => salary);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        string deptName;
        public string DeptName
        {
            get { return deptName; }
            set
            {
                deptName = value;
                NotifyOfPropertyChange(() => deptName);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }

        string destination;
        public string Destination
        {
            get => destination;

            set
            {
                destination = value;
                NotifyOfPropertyChange(() => destination);
                NotifyOfPropertyChange(() => CanSaveEmployee);
            }
        }
        private EmployeesModel selectedEmployee;
        public EmployeesModel SelectedEmployee
        {
            get { return selectedEmployee; }
            set
            {
                selectedEmployee = value;
                if (value != null)
                {
                    Id = value.Id;
                    EmpName = value.EmpName;
                    Salary = value.Salary;
                    DeptName = value.DeptName;
                    Destination = value.Destination;
                }
            }
        }

        public void NewEmployee()
        {
            Id = 0;
            EmpName = string.Empty;
            Salary = 0;
            DeptName = Destination = string.Empty;
        }

        public bool CanSaveEmployee
        {
            //버튼 활성/비활성화를 위한 속성 :: 이벤트 함수명 앞에 can을 추가하셔 property를 정의한다
            get {
                return !string.IsNullOrEmpty(EmpName) &&
                  !string.IsNullOrEmpty(DeptName) &&
                  !string.IsNullOrEmpty(Destination) &&
                  Salary != 0;
            }
        }
        public void SaveEmployee()
        {
            //버튼 이벤트를 위한 속성
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                if (Id == 0)
                    cmd.CommandText = @"INSERT INTO TblEmployees
                                               (EmpName
                                               , Salary
                                               , DeptName
                                               , Destination)
                                         VALUES
                                               (@EmpName
                                               , @Salary
                                               , @DeptName
                                               , @Destination)";
                else //Update일 경우
                    cmd.CommandText = @"UPDATE TblEmployees
                                           SET EmpName = @EmpName
                                              ,Salary = @Salary
                                              ,DeptName = @DeptName
                                              ,Destination = @Destination
                                         WHERE Id = @Id";

                SqlParameter parmEmpName = new SqlParameter("@EmpName", EmpName);
                SqlParameter parmSalary = new SqlParameter("@Salary", Salary);
                SqlParameter parmDeptName = new SqlParameter("@DeptName", DeptName);
                SqlParameter parmDestination = new SqlParameter("@Destination", Destination);
                
                cmd.Parameters.Add(parmEmpName);
                cmd.Parameters.Add(parmSalary);
                cmd.Parameters.Add(parmDeptName);
                cmd.Parameters.Add(parmDestination);

                if(Id != 0)
                {
                    //update일 경우 Id값도 파라미터로 추가해야함
                    SqlParameter parmId = new SqlParameter("@Id", Id);
                    cmd.Parameters.Add(parmId);

                }

                cmd.ExecuteNonQuery();

            }//end of using (SqlConnection)

            //입력창 전부 초기화
            NewEmployee();

            //데이터 다시 조회
            GetEmployees();

        }

        public bool CanDelEmployee
        {
            get { return (Id != 0); }
        }
        public void DelEmployee()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                //다 똑같은 방..법..!
                string strQuery = "DELETE FROM TblEmployees WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(strQuery, conn);
                SqlParameter parmId = new SqlParameter("@Id", Id);
                cmd.Parameters.Add(parmId);

                cmd.ExecuteNonQuery();

            }
            NewEmployee();  //입력창 초기화
            GetEmployees(); //데이터 그리도 재 호출
        }
    }
}
