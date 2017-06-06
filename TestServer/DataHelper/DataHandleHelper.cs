using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class DataHandleHelper
    {


        public static int getHeadData(byte[] data) {
            ByteBuffer buffer = new ByteBuffer(data);
            int HeadData = buffer.ReadInt();
            return HeadData;
        }

        public static string getStringParam(byte[] data) {
            ByteBuffer buffer = new ByteBuffer(data);
            int HeadData = buffer.ReadInt();
            string stringParam = buffer.ReadString();
            return stringParam;
        }

        public static int getIntParam(byte[] data) {
            ByteBuffer buffer = new ByteBuffer(data);
            int HeadData = buffer.ReadInt();
            int IntParam = buffer.ReadInt();
            return IntParam;
        }
    }
}
