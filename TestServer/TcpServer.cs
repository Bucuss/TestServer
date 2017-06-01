using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TestServer
{
    class TcpServer
    {
        #region Private var
        private TcpClient client;            
        private List<TcpClient> clients;
        private NetworkStream streamToClient;
        private const int BufferSize = 8192;
        private byte[] buffer;
        private List<int> GameState = new List<int>();
        #endregion

        #region Connect request param
        private const int LOGIN_REQUEST = 1;            //登陆请求
        private const int LOADING_REQUEST = 2;          //载入请求
        private const int UPDATE_REQUEST = 3;           //更新请求
        private const int MATCH_REQUEST = 4;            //匹配请求
        private const int BATTLE_REQUEST = 5;           //对战请求
        private const int IS_ONLINE = 6;                //检测在线请求
        #endregion

        #region Connect reply param
        private const int LOGIN_SUCCESS_REPLY = 1;      //登陆成功回应
        private const int LOGIN_FAIL_REPLY = 2;         //登陆失败回应
        private const int LOADING_SUCCESS_REPLY = 3;    //载入成功回应
        private const int UPDATE_FAIL_REPLY = 4;        //更新失败回应
        private const int MATCH_SUCCESS_REPLY = 5;      //匹配成功回应
        private const int MATCH_FAIL_REPLY = 6;         //匹配失败回应
        private const int BATTLE_REPLY = 7;             //对战回应
        #endregion

        #region Contrust funtion
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="client">连接的客户端</param>
        /// <param name="clients">储存的客户端链表</param>
        public TcpServer(TcpClient client,List<TcpClient> clients) {
            this.client = client;
            this.clients = clients;
            Console.WriteLine("\nClient Connected Local:{0} <-- Client:{1}", client.Client.LocalEndPoint, client.Client.RemoteEndPoint);
            streamToClient = client.GetStream();
            buffer = new byte[BufferSize];
            //开始回调接受消息
            AsyncCallback callback = new AsyncCallback(ReadComplete);
            streamToClient.BeginRead(buffer, 0, BufferSize, callback, null);
        }
        #endregion

        #region Private Method
        /// <summary>
        /// 消息接受，回调函数，执行以下功能
        /// 登陆验证
        /// 载入请求
        /// 匹配请求
        /// 发送指令
        /// </summary>
        /// <param name="ar">异步回调的结果</param>
        private void ReadComplete(IAsyncResult ar)
        {
            int byteRead = 0;
            byte[] temp;//临时变量
            try
            {
                byteRead = streamToClient.EndRead(ar);//接受消息长度

                if (byteRead == 0)
                {
                    Console.WriteLine("Client offine");
                    return;
                }
                switch (getRequest(buffer))
                {
                    case LOGIN_REQUEST:
                        //登录验证
                        UserInfo userInfo = new UserInfo();
                        bool isExitUser = ConectMySqldb.queryUserInfoByUserName(getUsername(buffer), userInfo);
                        if (isExitUser)
                        {
                            Console.Write("username: {0} ,password: {1} ", userInfo.username, userInfo.password);
                            temp = userInfo.WriteAsBytes(LOGIN_SUCCESS_REPLY);//返回登陆成功回应
                            streamToClient.Write(temp, 0, temp.Length);
                        }
                        else
                        {
                            sendOnlyReply(LOGIN_FAIL_REPLY, streamToClient);//返回失败成功回应
                        }
                        break;
                    case LOADING_REQUEST:
                        //载入请求
                        UserAndReviewInfo reviewInfo = new UserAndReviewInfo();
                        ConectMySqldb.queryReviewInfoByUserName(getUsername(buffer), reviewInfo);
                        temp = reviewInfo.WriteAsBytes(LOADING_SUCCESS_REPLY);//返回载入成功回应
                        streamToClient.Write(temp, 0, temp.Length);
                        Console.WriteLine("battleNum:{0},victoryNum:{1},escapeNum:{2},singlehighestScore:{3},totalScore:{4}", reviewInfo.battleNum, reviewInfo.victoryNum, reviewInfo.escapeNum, reviewInfo.singlehighestScore, reviewInfo.totalScore);
                        
                        break;
                    case UPDATE_REQUEST:
                        //更新请求
                        UserAndReviewInfo new_reviewInfo = new UserAndReviewInfo();
                        new_reviewInfo.ReadByBytes(buffer);
                        bool isUpdate = ConectMySqldb.updateReviewInfo(new_reviewInfo);
                        if (isUpdate)
                        {Console.WriteLine("更新成功"); }
                        else
                        { Console.WriteLine("更新失败");sendOnlyReply(UPDATE_FAIL_REPLY, streamToClient); }
                        break;

                    case MATCH_REQUEST:
                        //匹配请求
                        bool isready = getIsReady(buffer, GameState);
                        if (clients.Count < 2||!isready)
                        {
                            sendOnlyReply(MATCH_FAIL_REPLY,streamToClient);//返回匹配失败回应
                        }
                        else if (clients.Count == 2&&isready)
                        {
                            sendOnlyReply(MATCH_SUCCESS_REPLY, streamToClient);//返回匹配成功回应
                        }
                        else {
                            sendOnlyReply(MATCH_FAIL_REPLY, streamToClient);//返回匹配失败回应
                            return;
                        }
                        break;
                    case BATTLE_REQUEST:
                        //发送指令
                        if (clients.ElementAt(0) == this.client) {
                            sendToTargetClient(clients.ElementAt(1),buffer);
                            Console.WriteLine("send to {0}", clients.ElementAt(1).Client.RemoteEndPoint);
                        }
                        else if (clients.ElementAt(1) == this.client)
                        {
                            sendToTargetClient(clients.ElementAt(0), buffer);
                            Console.WriteLine("send to {0}", clients.ElementAt(0).Client.RemoteEndPoint);
                        }
                        break;
                    default:
                        sendOnlyReply(IS_ONLINE, streamToClient);//心跳机制
                        break;
                }

                AsyncCallback callback = new AsyncCallback(ReadComplete);
                streamToClient.BeginRead(buffer, 0, BufferSize, callback, null);
            }
            catch(Exception ex) {

                if (streamToClient != null)
                    streamToClient.Dispose();
                clients.Remove(client);
                client.Close();
                Console.Write(ex.Message);
            }

        }


        /// <summary>
        /// 获取请求名称
        /// </summary>
        /// <param name="result">接受的消息</param>
        /// <returns>返回请求名称</returns>
        private int getRequest(byte[] result) {
            ByteBuffer buffer = new ByteBuffer(result);
            int request = buffer.ReadInt();
            return request;
        }

        /// <summary>
        /// 从接受数据获取用户名
        /// 登陆和载入时调用
        /// </summary>
        /// <param name="result">返回的消息</param>
        /// <returns>返回用户名</returns>
        private string getUsername(byte[] result) {
            ByteBuffer buffer = new ByteBuffer(result);
            int request = buffer.ReadInt();
            string username = buffer.ReadString();
            return username;
        }

        /// <summary>
        /// 获取匹配状态，
        /// 请求匹配时调用
        /// </summary>
        /// <param name="result">接受的消息</param>
        /// <param name="ReadyList">准备队列</param>
        /// <returns></returns>
        private bool getIsReady(byte[] result,List<int> ReadyList) {
            bool isReady = false;
            ByteBuffer buffer = new ByteBuffer(result);
            int request = buffer.ReadInt();
            if (ReadyList.Count() < 2)
            {
                ReadyList.Add(request);
                isReady = false;
            }
            else if(ReadyList.Count()==2){
                isReady = true;
            }
            return isReady;
        }

        /// <summary>
        /// 接受战斗指令并发送到指定client，交换数据
        /// </summary>
        /// <param name="targetclient">目标客户端</param>
        /// <param name="result">接受消息</param>
        private void sendToTargetClient(TcpClient targetclient,byte[] result) {
            NetworkStream targetstream = targetclient.GetStream();//获取目标客户端的networkstream
            BatteInfo battleInfo = new BatteInfo();
            battleInfo.ReadByBytes(result);
            Console.WriteLine("battleInfo:dic:{0},attack:{1},dirc:{2}", battleInfo.dir,battleInfo.attack,battleInfo.dirc);
            byte[] temp = battleInfo.WriteAsBytes(BATTLE_REPLY);//返回战斗回应和数据
            targetstream.Write(temp, 0, temp.Length);
        }

        /// <summary>
        /// 单独发送回应
        /// </summary>
        /// <param name="REPLY">回应</param>
        /// <param name="ns">写入流</param>
        private void sendOnlyReply(int REPLY,NetworkStream ns) {
            ByteBuffer reply = new ByteBuffer();
            reply.WriteInt(REPLY);//返回回应
            ns.Write(reply.ToBytes(), 0, reply.ToBytes().Length);
        }
        #endregion

    }
}
