using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpSkills.WPF
{
    internal class ChangeUI
    {
        // WPF 에서 UI를 변경하고 싶을 때, UI Thread(WPF에서는 Main Thread)가 아닌 다른 Thread에서 컨트롤의 속성을 변경하면 데이터가 달라져 굉장히 위험한 코드가 된다.
        // 참고 : UI Thread 연동을 위한 팁 - https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=neos_rtos&logNo=220759112213

        public void ChangeUIAtAnotherThread()
        {
            Thread th = new Thread(ChangeSomething);
            th.Start();
        }

        private void ChangeSomething()
        {
            this.Dispatcher.Invoke(() => {
                textbox.Text = i.ToString();
            });
            // UI Change 
        }

    }
}
