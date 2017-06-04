using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    class Client
    {

        #region Private var
        private ReturnInfo returnInfos = new ReturnInfo();
        private const int BufferSize = 8196;
        private byte[] buffer;
        private TcpClient client;
        private NetworkStream streamToServer;
        ReceviceMsg receviceMsg = new ReceviceMsg();
        DealMsg dealMsg = new DealMsg();
        #endregion


        #region
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">远程ip地址</param>
        /// <param name="port">端口</param>
        public Client(string ip, int port) {
            IPAddress mIp = IPAddress.Parse(ip);
            IPEndPoint ip_end_point = new IPEndPoint(mIp, port);
            try {
                client = new TcpClient();
                client.Connect(ip_end_point);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Server Connected Local:{0} <-- Server:{1}", client.Client.LocalEndPoint, client.Client.RemoteEndPoint);
            buffer = new byte[BufferSize];
            streamToServer = client.GetStream();
        }
        #endregion

        #region Send Method
        /// <summary>
        /// 发送请求用户信息的请求
        /// </summary>
        /// 接受后返回UserInfo
        /// <param name="username">用户名</param>
        public void SendMsgAskUserInfo(string username)
        {
            int request = CommandParam.LOGIN_REQUEST;
            try
            {
                ByteBuffer sender = new ByteBuffer();
                sender.WriteInt(request);
                sender.WriteString(username);
                sender.Flush();
                streamToServer.Write(sender.ToBytes(), 0, sender.ToBytes().Length);
                Console.WriteLine("Send:{0} is success",username);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

        }

        /// <summary>
        /// 发送请求战斗信息的请求
        /// </summary>
        /// 接受后返回UserAndReviewInfo
        /// <param name="username">用户名</param>
        public void SendMsgAskReviewInfo(string username)
        {
            int request = CommandParam.LOADING_REQUEST;
            try
            {
                ByteBuffer sender = new ByteBuffer();
                sender.WriteInt(request);
                sender.WriteString(username);
                sender.Flush();
                streamToServer.Write(sender.ToBytes(), 0, sender.ToBytes().Length);
                Console.WriteLine("Send:{0} is success", username);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

        }

        /// <summary>
        /// 发送更新战斗信息的请求
        /// </summary>
        /// <param name="reviewInfo">战斗信息</param>
        public void SendMsgAskUpdateReviewInfo(UserAndReviewInfo reviewInfo)
        {
            int request = CommandParam.UPDATE_REQUEST;
            try
            {
                byte[] sender = reviewInfo.WriteAsBytes(CommandParam.UPDATE_REQUEST);
                streamToServer.Write(sender, 0, sender.Length);
                Console.WriteLine("Send:{0} is success", request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        /// <summary>
        /// 发送请求匹配的请求
        /// 接受后返回IsMatch
        /// </summary>
        public void SendMsgAskMatchInfo()
        {
            int request = CommandParam.MATCH_REQUEST;
            try
            {
                ByteBuffer sender = new ByteBuffer();
                sender.WriteInt(request);
                streamToServer.Write(sender.ToBytes(), 0, sender.ToBytes().Length);
                Console.WriteLine("Send:{0} is success", request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        /// <summary>
        /// 发送战斗指令
        /// </summary>
        /// <param name="battleInfo">战斗指令</param>
        public void SendMsgAskBattleInfo(BatteInfo battleInfo)
        {
            int request = CommandParam.BATTLE_REQUEST;
            try
            {
                byte[] sender = battleInfo.WriteAsBytes(CommandParam.BATTLE_REQUEST);
                streamToServer.Write(sender, 0, sender.Length);
                Console.WriteLine("Send:{0} is success", request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public void sendHeartbeat() {
            ByteBuffer sender = new ByteBuffer();
            sender.WriteInt(CommandParam.IS_ONLINE);
            streamToServer.Write(sender.ToBytes(), 0, sender.ToBytes().Length);
        }
        #endregion

        /// <summary>
        /// 接受数据的方法
        /// </summary>
        public void Receive() {

            receviceMsg.ReceviceEvent += dealMsg.doDealMsg;
            receviceMsg.doRecevice(buffer,streamToServer);
            Thread.Sleep(1000);
        }


        public UserInfo getReceiveUserInfo()
        {
            return dealMsg.getDealuserInfo();
        }
        public UserAndReviewInfo getReceiveReviewInfo() {

            return dealMsg.getDealuserAndReviewInfo();

        }
        public bool getReceiveIsMatch() {

            return dealMsg.getDealIsMatch();
        }

        public BatteInfo getReceiveBattleInfo() {
            return dealMsg.getDealbattleInfo();
        }

        public bool getReceiveIsUpdate() {
            return dealMsg.getDealIsUpdate();
        }


    }
}


