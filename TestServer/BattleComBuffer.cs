using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{   
    //临时战斗指令数据
    class BattleComBuffer
    {
        private int dirc;
        private int attack;
        private int dircContinues;

        public BattleComBuffer() {
            this.dirc = 0;
            this.attack = 0;
            this.dircContinues = 0;
        }
        public BattleComBuffer(int dirc,int attack,int dircContinues) {
            this.dirc = dirc;
            this.attack = attack;
            this.dircContinues = dircContinues;
        }

        public ByteBuffer WriteAsByte() {
            ByteBuffer bytebuffer = new ByteBuffer();
            bytebuffer.WriteInt(dirc);
            bytebuffer.WriteInt(attack);
            bytebuffer.WriteInt(dircContinues);
            return bytebuffer;
        }

        public void ReadByByte(byte[] result) {
            ByteBuffer bytebuffer = new ByteBuffer(result);
            this.dirc = bytebuffer.ReadInt();
            this.attack = bytebuffer.ReadInt();
            this.dircContinues = bytebuffer.ReadInt();
        }
    }
}
