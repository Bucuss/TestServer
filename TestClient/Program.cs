using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TestClient
{
    class Program
    {
        private const int port = 8088;
        private static string IpStr = "127.0.0.1";
        static void Main(string[] args)
        {
            Client c = new Client(IpStr,port);
            
            c.SendMsgAskUserInfo("zbm");
            c.SendMsgAskReviewInfo("zbm");
            c.SendMsgAskMatchInfo();
            c.Receive();
            UserInfo userInfo = new UserInfo();
            userInfo = c.getReceiveUserInfo();
            Console.WriteLine("userInfo: username:{0},password:{1}",userInfo.username,userInfo.password);
            UserAndReviewInfo reviewInfo = new UserAndReviewInfo();
            reviewInfo = c.getReceiveReviewInfo();
            Console.WriteLine("battleNum:{0},victoryNum:{1},escapeNum:{2},singlehighestScore:{3},totalScore:{4}", reviewInfo.battleNum, reviewInfo.victoryNum, reviewInfo.escapeNum, reviewInfo.singlehighestScore, reviewInfo.totalScore);
            bool isMatch = false;
            while (!isMatch) {
                Console.WriteLine("匹配失败");
                c.SendMsgAskMatchInfo();
                c.Receive();
                isMatch = c.getReceiveIsMatch();
                Console.WriteLine("IsMatch:{0}",isMatch);
            }

            Console.ReadKey();
        }

    }
}
