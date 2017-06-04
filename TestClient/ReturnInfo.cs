using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
   

    public class ReturnInfo
    {
        private const int UPDATE_FAIL_REPLY = 4;        //更新失败回应
        private const int MATCH_SUCCESS_REPLY = 5;      //匹配成功回应
        private const int MATCH_FAIL_REPLY = 6;         //匹配失败回应
        /// <summary>
        /// 返回用户信息
        /// </summary>
        /// <param name="result">接受消息</param>
        /// <returns>返回用户信息</returns>
        public UserInfo returnUserInfo(byte[] result)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.ReadByBytes(result);
            return userInfo;
        }

        /// <summary>
        /// 返回战斗信息
        /// </summary>
        /// <param name="result">接受消息</param>
        /// <returns>返回战斗信息</returns>
        public UserAndReviewInfo returnUserAndReviewInfo(byte[] result)
        {
            UserAndReviewInfo reviewInfo = new UserAndReviewInfo();
            reviewInfo.ReadByBytes(result);
            return reviewInfo;
        }

        /// <summary>
        /// 返回是否更新
        /// </summary>
        /// <param name="result">接受消息</param>
        /// <returns>返回是否更新</returns>
        public bool returnIsUpdate(byte[] result) {
            bool IsUpdate = true;
            ByteBuffer buffer = new ByteBuffer(result);
            int reply = buffer.ReadInt();
            if (reply == UPDATE_FAIL_REPLY)
            {
                IsUpdate = false;
            }
            else
            {
                IsUpdate = true;
            }
            return IsUpdate;
        }
        /// <summary>
        /// 返回匹配信息
        /// </summary>
        /// <param name="result">接受消息</param>
        /// <returns>返回是否匹配</returns>
        public bool returnIsMatchInfo(byte[] result)
        {
            bool IsMatch = false;
            ByteBuffer buffer = new ByteBuffer(result);
            int reply = buffer.ReadInt();
            if (reply == MATCH_SUCCESS_REPLY)
            {
                IsMatch = true;
            }
            else if (reply == MATCH_FAIL_REPLY)
            {
                IsMatch = false;
            }
            return IsMatch;
        }

        /// <summary>
        /// 返回战斗信息
        /// </summary>
        /// <param name="result">接受消息</param>
        /// <returns>返回战斗信息</returns>
        public BatteInfo returnBatteInfo(byte[] result)
        {
            BatteInfo battleInfo = new BatteInfo();
            battleInfo.ReadByBytes(result);
            Console.WriteLine("battleInfo:dic:{0},attack:{1},dirc:{2}", battleInfo.dir, battleInfo.attack, battleInfo.dirc);
            return battleInfo;
        }

        public bool returnIsOnline(byte[] result)
        {
            return true;
        }
    }
}
