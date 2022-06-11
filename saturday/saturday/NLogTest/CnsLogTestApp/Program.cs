using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace CnsLogTestApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Commons.LOGGER.Info("DataBase 접속 시도");

            string connString = "Data Source=PC01;Initial Catalog=OpenApiLab;Integrated Security=True";
            string strOuery = @"SELECT Id
                                      ,EmpName
                                      ,Salary
                                      ,DeptName
                                      ,Destination
                                  FROM TblEmployees";

            Commons.LOGGER.Info("DataBase 설정 및 쿼리 작성");
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(strOuery, conn);
                    Commons.LOGGER.Warn("접속실패가 발생 할 수 있습니다");

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                        Console.WriteLine(reader["EmpName"]);

                    }

                    Commons.LOGGER.Info("DB 처리 완료");
                }

                
            }
            catch (Exception ex)
            {

                Commons.LOGGER.Error($"예외발생! : {ex}");
                Console.WriteLine("예외발생 :: 관리자번호 010-0101-0000");

            }
            Commons.LOGGER.Info("DB 접속 종료");
        }
    }
}
