using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class test
    {
        private TcpListener listener;
        private byte[] buffer;
        private const int BufferSize = 2048;
        private Socket client;

        public test(string IpStr, int port) {
            IPAddress ip = IPAddress.Parse(IpStr);
            IPEndPoint ip_end_point = new IPEndPoint(ip, port);
            listener = new TcpListener(ip_end_point);
            listener.Start();
            
            listener.BeginAcceptSocket(new AsyncCallback(clientConnect), listener);
            
        }

        private void clientConnect(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener)ar.AsyncState;
                this.client = listener.EndAcceptSocket(ar);
                receviceData(client);
                
            }
            catch (Exception ex){
                Console.WriteLine(ex.Message);
            }
        }

        private void receviceData(Socket client) {
            client.BeginReceive(buffer, 0, BufferSize, SocketFlags.None,receviceCallback, buffer);

        }

        private void receviceCallback(IAsyncResult ar)
        {
            int receviceLen = 0;
            try
            {
                receviceLen = client.EndReceive(ar);
                if (receviceLen > 0)
                {
                    buffer = (byte[])ar.AsyncState;
                    OnReceiveData(client,buffer);
                }
                else {
                    Console.WriteLine("No Massage");
                }
            }
            catch {

            }
        }

        private void OnReceiveData(Socket client,byte[] buffers)
        {   

            ByteBuffer result = new ByteBuffer(buffers);
            string receviceStr = result.ReadString();
            Console.WriteLine("FromClient:"+receviceStr);

            ByteBuffer reply = new ByteBuffer();
            reply.WriteString("hello client");
            client.BeginSend(reply.ToBytes(),0, reply.ToBytes().Length,SocketFlags.None,sendCallBack, client);
        }

        private void sendCallBack(IAsyncResult ar)
        {
            client.EndSend(ar);
        }
    }
}
