using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace TestServer
{
    class Program
    {
        private static byte[] result = new byte[1024];
        private const int port = 8088;
        private static string IpStr = "127.0.0.1";
        private static Socket serverSocket;

        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse(IpStr);
            IPEndPoint ip_end_point = new IPEndPoint(ip, port);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ip_end_point);
            serverSocket.Listen(10);

            Console.WriteLine("Start Listen {0} success", serverSocket.LocalEndPoint.ToString());
            Thread thread = new Thread(ClientConnectListen);
            thread.Start();
            Console.ReadLine();
        }

        private static void ClientConnectListen() {
            while (true) {
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("Client {0} load success", clientSocket.RemoteEndPoint.ToString());
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteString("Connected Server");//return data
                clientSocket.Send(WriteMessage(buffer.ToBytes()));
                Thread thread = new Thread(RecieveMessage);
                thread.Start(clientSocket);
            }

        }

        private static byte[] WriteMessage(byte[] message)
        {
            MemoryStream ms = null;
            //using 强制资源清理
            using (ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter writer = new BinaryWriter(ms);
                ushort msglen = (ushort)message.Length;
                writer.Write(msglen);
                writer.Write(message);
                writer.Flush();
                return ms.ToArray();
            }
        }

        private static void RecieveMessage(object clientSocket) {
            Socket mClientSocket = (Socket)clientSocket;
            while (true) {
                try {
                    int receiveNumber = mClientSocket.Receive(result);
                    Console.WriteLine("Receive Client {0} massage , length is {1}", mClientSocket.RemoteEndPoint.ToString(), receiveNumber);
                    ByteBuffer buff = new ByteBuffer(result);

                    int len = buff.ReadShort();
                    String data = buff.ReadString();
                    Console.WriteLine("Data: {0}", data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    mClientSocket.Shutdown(SocketShutdown.Both);
                    mClientSocket.Close();
                    break;
                }
            }
        }
    }
}
