using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public class DealMsg

    {

        #region Delegate class
        delegate UserInfo ReturnUserInfoDelegate(byte[] result);                                //获得用户信息委托
        delegate UserAndReviewInfo ReturnUserAndReviewInfoDelegate(byte[] result);              //获得战斗信息委托
        delegate bool ReturnIsMatch(byte[] result);                                             //获得是否匹配委托
        delegate BatteInfo ReturnBattleInfo(byte[] result);                                     //获得战斗指令委托
        delegate bool ReturnIsUpdate(byte[] result);                                            //获得是否更新委托
        delegate bool ReturnIsOnline(byte[] result);                                            //获得是否在线委托
        #endregion

        #region 
        public delegate void LoginHandler(UserInfo userInfo);
        public event LoginHandler LoginEvent;//登陆事件

        public delegate void LoadHandler(UserAndReviewInfo reviewInfo);
        public event LoadHandler LoadEvent;//数据载入事件

        public delegate void MatchHandler(bool isMatch);
        public event MatchHandler MatchEvent;//匹配信息事件

        public delegate void BattleHandler(BatteInfo battleInfo);
        public event BattleHandler BattleEvent;//战斗指令事件
        #endregion

        #region init Data
        private ReturnUserInfoDelegate del_UserInfo;
        private ReturnUserAndReviewInfoDelegate del_UserAndReviewInfo;
        private ReturnIsMatch del_IsMatch;
        private ReturnBattleInfo del_BattleInfo;
        private ReturnIsUpdate del_IsUpdate;
        private ReturnIsOnline del_IsOnline;
        private ReturnInfo returnInfos = new ReturnInfo();

        private UserInfo userInfo = new UserInfo();
        private UserAndReviewInfo userAndReviewInfo = new UserAndReviewInfo();
        private bool IsMatch = false;
        private BatteInfo battleInfo = new BatteInfo();
        private bool IsUpdate = true;
        private bool IsOnline = true;
        #endregion

        /// <summary>
        /// 处理接受的数据
        /// </summary>
        /// <param name="buffer">接受的数据</param>
        public void doDealMsg(byte[] buffer) {

            IAsyncResult ar;
            ByteBuffer temp = new ByteBuffer(buffer);
            int reply = temp.ReadInt();
            switch (reply)
            {
                case CommandParam.LOGIN_SUCCESS_REPLY:
                    //登陆成功
                    del_UserInfo = new ReturnUserInfoDelegate(returnInfos.returnUserInfo);
                    ar = del_UserInfo.BeginInvoke(buffer, null, null);
                    userInfo = del_UserInfo.EndInvoke(ar);
                    if (LoginEvent != null)
                        LoginEvent(userInfo);
                    break;
                case CommandParam.LOGIN_FAIL_REPLY:
                    //登陆失败
                    userInfo = null;
                    if (LoginEvent != null)
                        LoginEvent(userInfo);
                    Console.WriteLine("用户不存在");
                    break;
                case CommandParam.LOADING_SUCCESS_REPLY:
                    //载入成功
                    del_UserAndReviewInfo = new ReturnUserAndReviewInfoDelegate(returnInfos.returnUserAndReviewInfo);
                    ar = del_UserAndReviewInfo.BeginInvoke(buffer, null, null);
                    userAndReviewInfo = del_UserAndReviewInfo.EndInvoke(ar);
                    if (LoadEvent != null)
                        LoadEvent(userAndReviewInfo);
                    break;
                case CommandParam.UPDATE_FAIL_REPLY:
                    //更新失败
                    del_IsUpdate = new ReturnIsUpdate(returnInfos.returnIsUpdate);
                    ar = del_IsUpdate.BeginInvoke(buffer, null, null);
                    IsUpdate = del_IsUpdate.EndInvoke(ar);
                    break;
                case CommandParam.MATCH_SUCCESS_REPLY:
                    //匹配成功
                    del_IsMatch = new ReturnIsMatch(returnInfos.returnIsMatchInfo);
                    ar = del_IsMatch.BeginInvoke(buffer, null, null);
                    IsMatch = del_IsMatch.EndInvoke(ar);
                    if (MatchEvent != null)
                        MatchEvent(IsMatch);
                    break;
                case CommandParam.MATCH_FAIL_REPLY:
                    //匹配失败
                    del_IsMatch = new ReturnIsMatch(returnInfos.returnIsMatchInfo);
                    ar = del_IsMatch.BeginInvoke(buffer, null, null);
                    IsMatch = del_IsMatch.EndInvoke(ar);
                    if (MatchEvent != null) MatchEvent(IsMatch);
                    break;
                case CommandParam.BATTLE_REPLY:
                    //战斗回应
                    del_BattleInfo = new ReturnBattleInfo(returnInfos.returnBatteInfo);
                    ar = del_BattleInfo.BeginInvoke(buffer, null, null);
                    battleInfo = del_BattleInfo.EndInvoke(ar);
                    if(BattleEvent!=null) BattleEvent(battleInfo);
                    break;
                default:
                    del_IsOnline = new ReturnIsOnline(returnInfos.returnIsOnline);
                    ar = del_IsOnline.BeginInvoke(buffer, null, null);
                    IsOnline = del_IsOnline.EndInvoke(ar);
                    break;
            }

        }

        public UserInfo getDealuserInfo() {
            return this.userInfo;
        }
        public UserAndReviewInfo getDealuserAndReviewInfo() {
            return this.userAndReviewInfo;
        }
        public BatteInfo getDealbattleInfo() {
            return this.battleInfo;
        }
        public bool getDealIsMatch() {
            return this.IsMatch;
        }
        public bool getDealIsUpdate() {
            return this.IsUpdate;
        }
        public bool getDealIsOnline() {
            return this.IsOnline;
        }
    }
}
 