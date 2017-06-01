using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace TestServer
{
    class Program
    {

        #region Private param
        private const int port = 8088;                 //端口号
        private static string IpStr = "127.0.0.1";     //远程地址
        #endregion


        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse(IpStr);
            IPEndPoint ip_end_point = new IPEndPoint(ip, port);
            TcpListener listener = new TcpListener(ip_end_point);
            List<TcpClient> clients = new List<TcpClient>();

            //开始监听
            listener.Start();
            Console.WriteLine("Server:{0},Start Listening....",listener.LocalEndpoint);

            while (true) {

                TcpClient client = listener.AcceptTcpClient();
                if (clients.Count == 0 && client != null)
                {
                    clients.Add(client);
                }
                else if (clients.Count > 0 && client != null)
                {
                    if (!clients.Contains(client))
                    {
                        clients.Add(client);
                    }
                }
                else {
                    return;
                }
                TcpServer wapper = new TcpServer(client, clients);
            }
        }

    }
}
