using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpSkills.Common
{
    // Linq를 사용해 보다 직관적이고 편리하게 데이터를 추출할 수 있다.
    internal class LinqExample
    {
        public List<string> SelectList(List<string> originList)
        {
            var selectList = (from str in originList
                where str.Contains("검색할문자")
                orderby str descending  // ascending 가나다순, descending 역순
                select str)
                .ToList();

            return selectList;
        }
    }
}
