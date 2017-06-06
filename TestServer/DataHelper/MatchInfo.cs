using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class MatchInfo:Ipack
    {
        public int isMatch { get; set; }
        public int battleCode { get; set; }

        public MatchInfo() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isMatch">是否匹配</param>
        /// <param name="clientLocalAddress">战斗编码</param>
        public MatchInfo(int isMatch,int battleCode) {
            this.isMatch = isMatch;
            this.battleCode = battleCode;
        }

        public byte[] WriteAsBytes(int HeadData)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(HeadData);
            buffer.WriteInt(this.isMatch);
            buffer.WriteInt(this.battleCode);
            buffer.Flush();
            return buffer.ToBytes();
        }

        public int ReadByBytes(byte[] result)
        {
            ByteBuffer buffer = new ByteBuffer(result);
            int HeadData = buffer.ReadInt();
            this.isMatch = buffer.ReadInt();
            this.battleCode = buffer.ReadInt();
            return HeadData;
        }


    }
}
