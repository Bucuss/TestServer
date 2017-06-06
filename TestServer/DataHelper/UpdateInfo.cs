using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class UpdateInfo : Ipack
    {
        public int isUpdate { get; set; }

        public UpdateInfo() {
            this.isUpdate = 1;
        }

        public UpdateInfo(int isUpdate) {
            this.isUpdate = isUpdate;
        }

        public int ReadByBytes(byte[] result)
        {
            ByteBuffer buffer = new ByteBuffer(result);
            int HeadData=buffer.ReadInt();
            this.isUpdate=buffer.ReadInt();
            return HeadData;
            throw new NotImplementedException();
        }

        public byte[] WriteAsBytes(int HeadData)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(HeadData);
            buffer.WriteInt(this.isUpdate);
            buffer.Flush();
            return buffer.ToBytes();
            throw new NotImplementedException();
        }
    }
}
