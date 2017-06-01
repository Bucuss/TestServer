using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class UserAndReviewInfo
    {
        #region Public param
        public string username { get; set; }                //用户名
        public int battleNum { get; set; }                  //战斗次数
        public int victoryNum { get; set; }                 //胜利次数
        public int escapeNum { get; set; }                  //逃跑次数
        public int singlehighestScore { get; set; }         //最高分
        public int totalScore { get; set; }                 //总分
        #endregion

        #region Construct funtion
        /// <summary>
        /// 无参构造
        /// </summary>
        public UserAndReviewInfo() { }

        /// <summary>
        /// //构造函数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="battleNum">战斗次数</param>
        /// <param name="victoryNum">胜利次数</param>
        /// <param name="escapeNum">逃跑次数</param>
        /// <param name="singlehighestScore">最高分</param>
        /// <param name="totalScore">总分</param>
        public UserAndReviewInfo(string username, int battleNum, int victoryNum, int escapeNum, int singlehighestScore, int totalScore) {
            this.username = username;
            this.battleNum = battleNum;
            this.victoryNum = victoryNum;
            this.escapeNum = escapeNum;
            this.singlehighestScore = singlehighestScore;
            this.totalScore = totalScore;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 将数据结构转换为二进制流
        /// </summary>
        /// <param name="RETURN">返回参数</param>
        /// 参数 RETURN 为 request or reply
        /// <returns>返回byte[]</returns>
        public byte[] WriteAsBytes(int RETURN) {

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(RETURN);
            buffer.WriteString(this.username);
            buffer.WriteInt(this.battleNum);
            buffer.WriteInt(this.victoryNum);
            buffer.WriteInt(this.escapeNum);
            buffer.WriteInt(this.singlehighestScore);
            buffer.WriteInt(this.totalScore);
            buffer.Flush();

            return buffer.ToBytes();
        }

        /// <summary>
        /// 将二进制流转换为数据结构
        /// </summary>
        /// <param name="result">接受数据</param>
        /// <returns>返回请求或回应</returns>
        public int ReadByBytes(byte[] result) {
            ByteBuffer buffer = new ByteBuffer(result);
            int RETURN = buffer.ReadInt();
            this.username = buffer.ReadString();
            this.battleNum = buffer.ReadInt();
            this.victoryNum = buffer.ReadInt();
            this.escapeNum = buffer.ReadInt();
            this.singlehighestScore = buffer.ReadInt();
            this.totalScore = buffer.ReadInt();
            return RETURN;
        }
        #endregion
    }
}
