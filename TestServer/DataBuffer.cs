using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    class DataBuffer
    {
        private string username;
        private string password;
        private int battleNum;
        private int victoryNum;
        private int escapeNum;
        private int singleHighestScore;
        private int totalScore;

        public DataBuffer() { }

        public DataBuffer(string username,string password,int battleNum,int victoryNum,int escapeNum,int singleHighestScore,int totalScore) {
            this.username = username;
            this.password = password;
            this.battleNum = battleNum;
            this.victoryNum = victoryNum;
            this.escapeNum = escapeNum;
            this.singleHighestScore = singleHighestScore;
            this.totalScore = totalScore;
        }



        public ByteBuffer WirteAsByte() {
            ByteBuffer bytebuffer = new ByteBuffer();
            bytebuffer.WriteString(username);
            bytebuffer.WriteString(password);
            bytebuffer.WriteInt(battleNum);
            bytebuffer.WriteInt(victoryNum);
            bytebuffer.WriteInt(escapeNum);
            bytebuffer.WriteInt(singleHighestScore);
            bytebuffer.WriteInt(totalScore);
            return bytebuffer;
        }

        public void ReadByByte(byte[] result)
        {
            ByteBuffer bytebuffer = new ByteBuffer(result);
            this.username = bytebuffer.ReadString();
            this.password = bytebuffer.ReadString();
            this.battleNum = bytebuffer.ReadInt();
            this.victoryNum = bytebuffer.ReadInt();
            this.escapeNum = bytebuffer.ReadInt();
            this.singleHighestScore = bytebuffer.ReadInt();
            this.totalScore = bytebuffer.ReadInt();
        }
    }
}
