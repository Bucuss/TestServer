using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    class ReceviceMsg
    {
        private byte[] result;                              //接受数据
        private int BufferSize = 8192;
        private NetworkStream streamToClient;               //连接服务端的流
        private TcpClient client;
        private List<TcpClient> clients;
        public delegate void ReceviceHandler(byte[] param, NetworkStream stream, TcpClient client, List<TcpClient> clients); //声明委托
        public event ReceviceHandler ReceviceEvent;         //声明事件

        public void doRecevice(byte[] buffer, NetworkStream stream,TcpClient client,List<TcpClient> clients)
        {
            this.result = buffer;
            this.streamToClient = stream;
            this.client = client;
            this.clients = clients;
            AsyncCallback callback = new AsyncCallback(ReadComplete);
            streamToClient.BeginRead(buffer, 0, BufferSize, callback, null);

        }

        private void ReadComplete(IAsyncResult ar)
        {
            int byteRead = 0;
            try
            {
                byteRead = streamToClient.EndRead(ar);
                if (byteRead == 0)
                {
                    Console.WriteLine("NO MSG");
                    return;
                }
                else
                {
                    if (ReceviceEvent != null)
                    {
                        ReceviceEvent(result, streamToClient, client, clients);
                       
                    }
                }
                AsyncCallback callback = new AsyncCallback(ReadComplete);
                streamToClient.BeginRead(result, 0, BufferSize, callback, null);
            }
            catch (Exception ex)
            {
                if(streamToClient != null)
                {
                    streamToClient.Dispose();
                    clients.Remove(client);
                }
                client.Close();
                Console.Write(ex.Message);

            }
        }
    }
}
