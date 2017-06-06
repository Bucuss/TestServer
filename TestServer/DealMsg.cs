using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class DealMsg
    {
        #region Private var
        private List<int> GameState = new List<int>();
        private Dictionary<TcpClient, string> Name2Client = new Dictionary<TcpClient, string>();
        #endregion


        public void doDealMsg(byte[] buffer,NetworkStream streamToClient, TcpClient client, List<TcpClient> clients)
        {
            byte[] temp;//临时变量
            Console.WriteLine(getRequest(buffer));
            switch (getRequest(buffer))
            {

                case CommandParam.LOGIN_REQUEST:
                    //登录验证
                    UserInfo userInfo = new UserInfo();
                    bool isExitUser = ConectMySqldb.queryUserInfoByUserName(getUsername(buffer), userInfo);
                    if (isExitUser)
                    {
                        Console.Write("\nusername: {0} ,password: {1} ", userInfo.username, userInfo.password);
                        temp = userInfo.WriteAsBytes(CommandParam.LOGIN_SUCCESS_REPLY);//返回登陆成功回应
                        try
                        {
                            Name2Client.Add(client, userInfo.username);//绑定用户名和客户端
                            streamToClient.Write(temp, 0, temp.Length);
                            Console.WriteLine("ClientsCount:{0},Client:{1},username:{2}", Name2Client.Count, Name2Client.ElementAt(0).Key.Client.RemoteEndPoint, Name2Client.ElementAt(0).Value);
                        }
                        catch (ArgumentException ex)
                        {
                            sendOnlyReply(CommandParam.LOGIN_FAIL_REPLY, streamToClient);
                            Console.WriteLine("此用户已经存在：" + ex.Message);
                        }
                    }
                    else
                    {
                        sendOnlyReply(CommandParam.LOGIN_FAIL_REPLY, streamToClient);//返回失败成功回应
                    }
                    break;
                case CommandParam.LOADING_REQUEST:
                    //载入请求
                    ReviewInfo reviewInfo = new ReviewInfo();
                    ConectMySqldb.queryReviewInfoByUserName(getUsername(buffer), reviewInfo);
                    temp = reviewInfo.WriteAsBytes(CommandParam.LOADING_SUCCESS_REPLY);//返回载入成功回应
                    streamToClient.Write(temp, 0, temp.Length);
                    Console.WriteLine("\nbattleNum:{0},victoryNum:{1},escapeNum:{2},singlehighestScore:{3},totalScore:{4}", reviewInfo.battleNum, reviewInfo.victoryNum, reviewInfo.escapeNum, reviewInfo.singlehighestScore, reviewInfo.totalScore);

                    break;
                case CommandParam.UPDATE_REQUEST:
                    //更新请求
                    ReviewInfo new_reviewInfo = new ReviewInfo();
                    new_reviewInfo.ReadByBytes(buffer);
                    bool isUpdate = ConectMySqldb.updateReviewInfo(new_reviewInfo);
                    if (isUpdate)
                    { Console.WriteLine("更新成功"); }
                    else
                    { Console.WriteLine("更新失败"); sendOnlyReply(CommandParam.UPDATE_FAIL_REPLY, streamToClient); }
                    break;

                case CommandParam.MATCH_REQUEST:
                    //匹配请求
                    Console.WriteLine("yes");
                    bool isready = getIsReady(buffer, GameState);
                    if (clients.Count < 2 || !isready)
                    {
                        sendOnlyReply(CommandParam.MATCH_FAIL_REPLY, streamToClient);//返回匹配失败回应
                    }
                    else if (clients.Count == 2 && isready)
                    {
                        sendOnlyReply(CommandParam.MATCH_SUCCESS_REPLY, streamToClient);//返回匹配成功回应
                    }
                    else
                    {
                        sendOnlyReply(CommandParam.MATCH_FAIL_REPLY, streamToClient);//返回匹配失败回应
                                                                                     //return;
                    }
                    break;
                case CommandParam.BATTLE_REQUEST:
                    //发送指令
                    if (clients.ElementAt(0) == client)
                    {
                        sendToTargetClient(clients.ElementAt(1), buffer);
                        Console.WriteLine("send to {0}", clients.ElementAt(1).Client.RemoteEndPoint);
                    }
                    else if (clients.ElementAt(1) == client)
                    {
                        sendToTargetClient(clients.ElementAt(0), buffer);
                        Console.WriteLine("send to {0}", clients.ElementAt(0).Client.RemoteEndPoint);
                    }
                    break;
                default:
                    sendOnlyReply(CommandParam.IS_ONLINE, streamToClient);//心跳机制
                    break;
            }

        }

        /// <summary>
        /// 获取请求名称
        /// </summary>
        /// <param name="result">接受的消息</param>
        /// <returns>返回请求名称</returns>
        private int getRequest(byte[] result)
        {
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
        private string getUsername(byte[] result)
        {
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
        private bool getIsReady(byte[] result, List<int> ReadyList)
        {
            bool isReady = false;
            ByteBuffer buffer = new ByteBuffer(result);
            int request = buffer.ReadInt();
            if (ReadyList.Count() < 2)
            {
                ReadyList.Add(request);
                isReady = false;
            }
            else if (ReadyList.Count() == 2)
            {
                isReady = true;
            }
            return isReady;
        }

        /// <summary>
        /// 接受战斗指令并发送到指定client，交换数据
        /// </summary>
        /// <param name="targetclient">目标客户端</param>
        /// <param name="result">接受消息</param>
        private void sendToTargetClient(TcpClient targetclient, byte[] result)
        {
            NetworkStream targetstream = targetclient.GetStream();//获取目标客户端的networkstream
            BattleInfo battleInfo = new BattleInfo();
            battleInfo.ReadByBytes(result);
            Console.WriteLine("battleInfo:dic:{0},attack:{1},dirc:{2}", battleInfo.dir, battleInfo.attack, battleInfo.dirc);
            byte[] temp = battleInfo.WriteAsBytes(CommandParam.BATTLE_REPLY);//返回战斗回应和数据
            targetstream.Write(temp, 0, temp.Length);
        }

        /// <summary>
        /// 单独发送回应
        /// </summary>
        /// <param name="REPLY">回应</param>
        /// <param name="ns">写入流</param>
        private void sendOnlyReply(int REPLY, NetworkStream ns)
        {
            ByteBuffer reply = new ByteBuffer();
            reply.WriteInt(REPLY);//返回回应
            ns.Write(reply.ToBytes(), 0, reply.ToBytes().Length);
        }


    }
}

