using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class UserInfo:Ipack
    {
        #region
        public string username { get; set; }    //用户名
        public string password { get; set; }    //密码
        #endregion

        #region Construt funtion
        /// <summary>
        /// 无参构造
        /// </summary>
        public UserInfo() {
            this.username = "";
            this.password = "";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public UserInfo(string username, string password) {
            this.username = username;
            this.password = password;
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 写入二进制流
        /// </summary>
        /// <param name="HeadData">返回参数</param>
        /// <returns>byte[]</returns>
        public byte[] WriteAsBytes(int HeadData)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(HeadData);
            buffer.WriteString(this.username);
            buffer.WriteString(this.password);
            buffer.Flush();
            return buffer.ToBytes();
        }

        /// <summary>
        /// 将二进制流读取成数据结构
        /// </summary>
        /// <param name="result">接受数据</param>
        /// <returns>返回请求或回应</returns>
        public int ReadByBytes(byte[] result)
        {
            ByteBuffer buffer = new ByteBuffer(result);
           // int len = buffer.ReadShort();
            int HeadData = buffer.ReadInt();
            this.username = buffer.ReadString();
            this.password = buffer.ReadString();
            return HeadData;
        }
        #endregion
    }
}
