using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpSkills.Common
{
    internal class DiscardExample
    {
        // 변수가 null인지 체크하기 위해 '_(독립형 폐기)'를 사용할 수 있다.
        public static void StandaloneDiscard(string arg)
        {
            _ = arg ?? throw new ArgumentNullException(paramName: nameof(arg), message: "arg can't be null");

        }

        // 파라미터 인자값에 out이 필요한데, 이 값을 사용하지 않을 때 버리는 용도로 사용할 수 있다.
        public static void OutParameter(string arg)
        {
            string[] dateStrings = {"05/01/2018 14:57:32.8", "2018-05-01 14:57:32.8",
                "2018-05-01T14:57:32.8375298-04:00", "5/01/2018",
                "5/01/2018 14:57:32.80 -07:00",
                "1 May 2018 2:57:32.8 PM", "16-05-2018 1:00:32 PM",
                "Fri, 15 May 2018 20:10:57 GMT" };
            foreach (string dateString in dateStrings)
            {
                if (DateTime.TryParse(dateString, out _))
                    Console.WriteLine($"'{dateString}': valid");
                else
                    Console.WriteLine($"'{dateString}': invalid");
            }
        }

        // switch 구문 사용할 때 default 처럼 사용할 수 있다.
        static void ProvidesFormatInfo(object? obj) =>
            Console.WriteLine(obj switch
            {
                IFormatProvider fmt => $"{fmt.GetType()} object",
                null => "A null object reference: Its use could result in a NullReferenceException",
                _ => "Some object type without format information"
            });
    }
}
