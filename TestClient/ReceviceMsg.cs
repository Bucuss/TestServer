using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    public class ReceviceMsg
    {
        private byte[] result;                              //接受数据
        private int BufferSize = 8192;
        private NetworkStream streamToServer;               //连接服务端的流
        public delegate void ReceviceHandler(byte[] param); //声明委托
        public event ReceviceHandler ReceviceEvent;         //声明事件

        public void doRecevice(byte[] buffer,NetworkStream stream) {
            this.result = buffer;
            this.streamToServer = stream;
            AsyncCallback callback = new AsyncCallback(ReadComplete);
            streamToServer.BeginRead(buffer, 0, BufferSize, callback, null);

        }

        private void ReadComplete(IAsyncResult ar) {
            int byteRead = 0;
            byteRead = streamToServer.EndRead(ar);
            if (byteRead == 0)
            {
                Console.WriteLine("NO MSG");
                return;
            }
            else {
                if (ReceviceEvent != null)
                {
                    ReceviceEvent(result);
                    AsyncCallback callback = new AsyncCallback(ReadComplete);
                    streamToServer.BeginRead(result, 0, BufferSize, callback, null);
                }
            }


        }
    }
}
