using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using TestServer.Test;


namespace TestServer
{
    class Program
    {

        #region
        //#region Private param
        //private const int port = 8088;                 //端口号
        //private static string IpStr = "0.0.0.0";     //远程地址
        //#endregion


        //static void Main(string[] args)
        //{
        //    IPAddress ip = IPAddress.Parse(IpStr);
        //    IPEndPoint ip_end_point = new IPEndPoint(ip, port);
        //    TcpListener listener = new TcpListener(ip_end_point);
        //    List<TcpClient> clients = new List<TcpClient>();

        //    //开始监听
        //    //listener.Start();
        //    test listeners = new test(IpStr, port);
        //    Console.WriteLine("Server:{0},Start Listening....",listener.LocalEndpoint);
        //    Console.ReadKey();

        //    //while (true) {

        //       // TcpClient client = listener.AcceptTcpClient();
        //        //test wapper = new test(client);
        //        //if (clients.Count == 0 && client != null)
        //        // {
        //        //     clients.Add(client);
        //        // }
        //        // else if (clients.Count > 0 && client != null)
        //        // {
        //        //    if (!clients.Contains(client))
        //        //     {
        //        // clients.Add(client);
        //        //     }
        //        // }
        //        //  else
        //        //  {
        //        //      return;
        //        //  }
        //        //TcpServer wapper = new TcpServer(client, clients,out clients);

        //   // }
        //}
        #endregion


        static AsyncTcpServer server;
        private static Dictionary<TcpClient,string> onLineList = new Dictionary<TcpClient,string>();//在线列表
        private static List<TcpClient> linelist = new List<TcpClient>();//匹配列表
        private static List<TcpClient[]> battleList = new List<TcpClient[]>();//对战列表

        static void Main(string[] args)
        {   
            server = new AsyncTcpServer(8088);
            server.Encoding = Encoding.UTF8;
            //注册事件
            server.ClientConnected +=
              new EventHandler<TcpClientConnectedEventArgs>(server_ClientConnected);                
            server.ClientDisconnected +=
              new EventHandler<TcpClientDisconnectedEventArgs>(server_ClientDisconnected);
            server.DatagramReceived +=
              new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(server_DatagramReceived);
            server.Start();
            Console.WriteLine("TCP server has been started.");
            Console.WriteLine("Type something to send to client...");
            while (true)
            {
                string text = Console.ReadLine();
                server.SendAll(text);
            }
        }
        /// <summary>
        /// 客户端连接事件执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">客户端连接事件参数-Tcpclient</param>
        static void server_ClientConnected(object sender, TcpClientConnectedEventArgs e)
        {
            Console.WriteLine("TCP client {0} has connected.", e.TcpClient.Client.RemoteEndPoint.ToString());
        }
        /// <summary>
        /// 客户端断开事件执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">客户端连接事件参数-Tcpclient</param>
        /// 
        static void server_ClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
        {
            if (onLineList.ContainsKey(e.TcpClient))
            {
                onLineList.Remove(e.TcpClient);//移除在线列表
                linelist.Remove(e.TcpClient);
                battleList.Clear();
            }
            Console.WriteLine("TCP client {0} has disconnected.", e.TcpClient.Client.RemoteEndPoint.ToString());
        }
        /// <summary>
        /// 服务器接受客户端消息时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">接受数据参数-Tcpclient,byte[]</param>
        static void server_DatagramReceived(object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            //TODO
            int headData = DataHandleHelper.getHeadData(e.Datagram);
            byte[] receviceData = e.Datagram;
            string username;
            bool isExist;
            int battleCode;

            switch (headData) {
                case CommandParam.LOGIN_REQUEST:
                    //登陆请求
                    username = DataHandleHelper.getStringParam(receviceData);
                    UserInfo userInfo = new UserInfo();
                    isExist = ConectMySqldb.queryUserInfoByUserName(username, userInfo);
                    if (isExist)
                    {
                        if (onLineList.ContainsKey(e.TcpClient) || onLineList.ContainsValue(username))
                        {
                            userInfo = new UserInfo();//已存在用户或客户端，返回空值
                        }
                        else
                        {
                            onLineList.Add(e.TcpClient, username);
                        }
                    }
                    else {
                        userInfo = new UserInfo();
                    }
                    //返回数据
                    server.Send(e.TcpClient, userInfo, CommandParam.LOGIN_RETURN);
                    break;
                case CommandParam.LOADING_REQUEST:
                    //载入请求
                    username = DataHandleHelper.getStringParam(receviceData);
                    ReviewInfo reviewInfo = new ReviewInfo();
                    isExist = ConectMySqldb.queryReviewInfoByUserName(username,reviewInfo);
                    if (!isExist) {
                        reviewInfo = new ReviewInfo();//不存在，返回空值
                    }
                    //返回数据
                    server.Send(e.TcpClient, reviewInfo, CommandParam.LOAD_RETURN);
                    break;
                case CommandParam.MATCH_REQUEST:
                    //匹配请求
                    
                    MatchInfo matchInfo = new MatchInfo();
                    if (linelist.Count == 0)
                    {
                        linelist.Add(e.TcpClient);
                        matchInfo.isMatch = 0;
                        matchInfo.battleCode = -1;
                    }
                    else if (linelist.Count == 1)
                    {
                        if (linelist.Contains(e.TcpClient))
                        {
                            matchInfo.isMatch = 0;
                            matchInfo.battleCode = -1;
                        }
                        else
                        {
                            TcpClient emeny = linelist.ElementAt(0);
                            linelist.Remove(emeny);//移除第一个匹配的元素
                            TcpClient[] battle = new TcpClient[] { emeny, e.TcpClient };
                            battleList.Add(battle);
                            matchInfo.isMatch = 1;
                            matchInfo.battleCode = battleList.IndexOf(battle);//在对战列表的位置
                        }
                    }
                    else {
                        matchInfo.isMatch = 0;
                        matchInfo.battleCode = -1;
                    }
                    //返回数据
                    server.Send(e.TcpClient, matchInfo, CommandParam.MATCH_RETURN);
                    break;
                case CommandParam.BATTLE_REQUEST:
                    //战斗数据交换
                    battleCode = DataHandleHelper.getIntParam(receviceData);
                    BattleInfo battleInfo = new BattleInfo();
                    battleInfo.ReadByBytes(receviceData);
                    
                    if (battleCode < 0)
                    {
                        return;//找不到战斗列表
                    }
                    else {
                        foreach (var p in battleList.ElementAt(battleCode)) {
                            if (p != e.TcpClient) {
                                TcpClient target = p;
                                battleInfo.setBattelCode(battleCode);
                                server.Send(target, battleInfo, CommandParam.BATTLE_RETURN);
                            }
                         }
                    } 
                    break;
                case CommandParam.UPDATE_REQUEST:
                    //更新数据        
                    ReviewInfo updatereviewInfo = new ReviewInfo();
                    updatereviewInfo.ReadByBytes(receviceData);
                    UpdateInfo updateInfo = new UpdateInfo();
                    bool isUpdate = ConectMySqldb.updateReviewInfo(updatereviewInfo);
                    if (isUpdate)
                    {
                        updateInfo.isUpdate = 1;
                    }
                    else {
                        updateInfo.isUpdate = 0;
                    }
                    //返回数据
                    server.Send(e.TcpClient, updateInfo, CommandParam.UPDATE_RETURN);
                    break;
                case CommandParam.FINSH_REQUEST:
                    battleCode = DataHandleHelper.getIntParam(receviceData);
                    if (battleList.ElementAt(battleCode) != null) {
                        battleList.RemoveAt(battleCode);
                    }
                    server.Send(e.TcpClient, "Gamerover");
                    break;
                default:
                    server.Send(e.TcpClient,"参数错误");
                    break;
            }


        }
    }


}
