using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WpfMvvmApp.Helpers
{
    public class Commons
    {
        /// <summary>
        /// 이메일 검증
        /// </summary>
        /// <param name="email"></param>
        /// <returns> true : 이메일 아님 </returns>
        public static bool IsValidEmail(string email)
        {
            //c# verbatim :: 축자 문자열 사용에서 기대되는 동작대로, 역슬래시 \ 는 escape 문자로 취급되지 않는다. 또한 보간 문자열 사용으로 인해 기대되는 동작대로, 중괄호로 둘러싸여진 표현식은 해당 위치에 문자열로 삽입되기 전에 실제 값이 평가 (evaluate) 된다.
            return Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
        }

        /// <summary>
        /// 나이계산
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CalcAge(DateTime value)
        {
            int middle;
            if (DateTime.Now.Month < value.Month || DateTime.Now.Month == value.Month && DateTime.Now.Day < value.Day)
                middle = DateTime.Now.Year - value.Year - 1;
            else
                middle = DateTime.Now.Year - value.Year;

            return middle;


        }
    }
}
