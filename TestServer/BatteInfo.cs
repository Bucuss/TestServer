using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    class BatteInfo
    {
        #region PUblic param
        public int dir { get; set; }            //方向
        public int attack { get; set; }         //是否攻击
        public int dirc { get; set; }           //是否持续移动
        #endregion

        #region Contrust funtion
        /// <summary>
        /// 无参构造
        /// </summary>
        public BatteInfo() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dir">方向</param>
        /// <param name="attack">是否攻击</param>
        /// <param name="dirc">是否持续移动</param>
        /// 特别的，参数attack和dirc取值为(0,1)
        public BatteInfo(int dir,int attack,int dirc) {
            this.dir = dir;
            this.attack = attack;
            this.dirc = dirc;
        }
        #endregion

        #region
        /// <summary>
        /// 写入二进制流
        /// </summary>
        /// <param name="RETURN">返回的参数值</param>
        /// <returns>byte[]</returns>
        public byte[] WriteAsBytes(int RETURN)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(RETURN);
            buffer.WriteInt(this.dir);
            buffer.WriteInt(this.attack);
            buffer.WriteInt(this.dirc);
            buffer.Flush();
            return buffer.ToBytes();
        }

        /// <summary>
        /// 将二进制流读取为数据结构
        /// </summary>
        /// <param name="result">接受数据</param>
        /// <returns>返回请求或回应</returns>
        public int ReadByBytes(byte[] result)
        {
            ByteBuffer buffer = new ByteBuffer(result);
            int RETURN = buffer.ReadInt();
            this.dir = buffer.ReadInt();
            this.attack = buffer.ReadInt();
            this.dirc = buffer.ReadInt();
            return RETURN;
        }
        #endregion
    }
}
