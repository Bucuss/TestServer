using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public class UserInfo
    {

        #region
        public string username { get; set; }         //用户名
        public string password { get; set; }         //密码
        #endregion

        #region Contrust funtion

        /// <summary>
        /// 无参构造
        /// </summary>
        public UserInfo() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public UserInfo(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
        #endregion

        /// <summary>
        /// 写入二进制流
        /// </summary>
        /// <param name="RETURN">请求或回应</param>
        /// <returns>byte[]</returns>
        public byte[] WriteAsBytes(int RETURN)
        {

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(RETURN);
            buffer.WriteString(this.username);
            buffer.WriteString(this.password);
            buffer.Flush();
            return buffer.ToBytes();
        }

        /// <summary>
        /// 读取二进制流为数据结构
        /// </summary>
        /// <param name="result">接受数据</param>
        /// <returns>返回请求或回应</returns>
        public int ReadByBytes(byte[] result)
        {
            ByteBuffer buffer = new ByteBuffer(result);
            int RETURN = buffer.ReadInt();
            this.username = buffer.ReadString();
            this.password = buffer.ReadString();
            return RETURN;
        }
    }
}
