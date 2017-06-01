using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace TestServer
{
    public  class ConectMySqldb
    {
        private static string conn = MySqlHelper.getConnection();

        /// <summary>
        /// 通过username查询用户账号密码
        /// </summary>
        /// <param name="username">string类型</param>
        /// <param name="uarInfo">UserInfo类型</param>
        /// 参数类型 MySqlParameter("@username","xxx")
        /// <returns>返回值，是否已存在用户名</returns>
        public static bool queryUserInfoByUserName(string username,UserInfo uarInfo) {
            bool isExist = true;
            string queryStr = "select * from users where username = @username";
            MySqlParameter param = new MySqlParameter("@username", username);
            MySqlDataReader msReader = MySqlHelper.ExecuteReader(conn, CommandType.Text, queryStr, param);
            if (msReader.Read() == true)
            {
                //储存数据
                uarInfo.username = msReader["username"].ToString();
                uarInfo.password = msReader["passwords"].ToString();
                isExist = true;
            }
            else {
                isExist = false;
            }
            return isExist;
        }

        /// <summary>
        /// 通过username查询战斗记录
        /// </summary>
        /// <param name="username">string类型</param>
        /// <param name="uarInfo">UserAndReviewInfo类型</param>
        /// 参数类型 MySqlParameter("@username","xxx")
        public static void queryReviewInfoByUserName(string username, UserAndReviewInfo uarInfo) {
            string queryStr = "select * from battlereview where username = @username";
            MySqlParameter param = new MySqlParameter("@username", username);
            MySqlDataReader msReader = MySqlHelper.ExecuteReader(conn, CommandType.Text, queryStr, param);
            if (msReader.Read() == true)
            {
                //储存数据
                uarInfo.username = msReader["username"].ToString();
                uarInfo.battleNum = Convert.ToInt32(msReader["battleNum"].ToString());
                uarInfo.victoryNum = Convert.ToInt32(msReader["victoryNum"].ToString());
                uarInfo.escapeNum = Convert.ToInt32(msReader["escapeNum"].ToString());
                uarInfo.singlehighestScore = Convert.ToInt32(msReader["singlehighestScore"].ToString());
                uarInfo.totalScore = Convert.ToInt32(msReader["totalScore"].ToString());


            }
        }

        /// <summary>
        /// 插入用户信息，实现注册功能
        /// </summary>
        /// <param name="username">string类型</param>
        /// <param name="password">string类型</param>
        /// 参数类型 MySqlParameter[] param = {new MySqlParameter("@username",xxx),new MySqlParameter("@password",xxx) }
        /// <returns>返回值，是否插入成功</returns>
        public static bool insertUserInfo(string username,string password) {

            bool isExist = false;//检测用户是否存在
            bool isinsert = false;//返回值，是否载入成功
            string insertStr = "insert into users (username,passwords) values (@username,@password)";
            string queryStr = "select username from users";
            MySqlParameter[] param = { new MySqlParameter("@username", username), new MySqlParameter("@password", password) };

            MySqlDataReader msReader = MySqlHelper.ExecuteReader(conn, CommandType.Text, queryStr, param[0]);
            if (msReader.Read() == true)
            {
                string getusername = msReader["username"].ToString();
                if (getusername.Equals(param[0].Value.ToString())) {

                    isExist = true;
                }
            }           
            if (isExist == true)
            {
                MySqlHelper.ExecuteNonQuery(conn, CommandType.Text, insertStr, param);
                isinsert = true;
                insertReviewInfo(param[0]);
                //初始化战斗信息             
            }
            else
            {
                isinsert = false;
            }
            return isinsert;
        }

        /// <summary>
        /// 完成注册后初始化战斗信息，在注册完成用户时调用
        /// </summary>
        /// 参数具体为用户名username
        /// <param name="usernameParam">MySqlParameter类型</param>
        private static void insertReviewInfo(MySqlParameter usernameParam) {
            MySqlParameter[] param = { new MySqlParameter("@battleNum", 1), new MySqlParameter("@victoryNum", 1), new MySqlParameter("@escapeNum", 1), new MySqlParameter("@singlehighestScore", 1), new MySqlParameter("@totalScore", 1), usernameParam };
            string insertStr = "insert into battlereview (battleNum,victoryNum,escapeNum,singlehighestScore,totalScore,username) values (@battleNum,@victoryNum,@escapeNum,@singlehighestScore,@totalScore,@username)";
            MySqlHelper.ExecuteNonQuery(conn, CommandType.Text, insertStr, param);

        }

        /// <summary>
        /// 更新用户信息，修改密码
        /// </summary>
        /// <param name="new_password">string类型 新的密码</param>
        /// <param name="username">string类型 原来的用户名</param>
        /// 参数类型 MySqlParameter[] param = {new MySqlParameter("@new_password",xxx),new MySqlParameter("@username",xxx) };
        /// <returns>返回值，是否更新成功</returns>
        public static bool updateUserInfo(string new_password,string username) {
            string updateStr = "update users set passwords = @new_password where username = @username";
            MySqlParameter[] param = { new MySqlParameter("@new_password", new_password), new MySqlParameter("@username", username) };
            MySqlHelper.ExecuteNonQuery(conn, CommandType.Text, updateStr, param);
            return true;
        }


        /// <summary>
        /// 更新战斗数据
        /// </summary>
        /// <param name="new_reviewInfo">UserAndReviewInfo类型</param>
        /// <returns>返回值，是否更新成功</returns>
        public static bool updateReviewInfo(UserAndReviewInfo new_reviewInfo)
        {   
            string updateStr = "update users set battleNum = @new_battleNum,victoryNum = @new_victoryNum,escapeNum = @new_escapeNum,singlehighestScore = @new_singlehighestScore,totalScore = @new_totalScore  where username = @username";
            MySqlParameter[] param = { new MySqlParameter("@new_battleNum", new_reviewInfo.battleNum), new MySqlParameter("@new_victoryNum", new_reviewInfo.victoryNum), new MySqlParameter("@new_escapeNum", new_reviewInfo.escapeNum), new MySqlParameter("@new_singlehighestScore", new_reviewInfo.singlehighestScore), new MySqlParameter("@new_totalScore", new_reviewInfo.totalScore), new MySqlParameter("@username", new_reviewInfo.username) };
            MySqlHelper.ExecuteNonQuery(conn, CommandType.Text, updateStr, param);
            return true;
        }

        /// <summary>
        /// 删除用户所有信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// 参数类型 MySqlParameter param = new MySqlParameter("@username","username")
        /// <returns>返回值，是否删除完成</returns>
        public static bool deleteUserAllInfo(string username) {
            string deleteStr = "delete from users where username = @username";
            MySqlParameter param = new MySqlParameter("@username", username);
            MySqlHelper.ExecuteNonQuery(conn, CommandType.Text, deleteStr, param);
            deleteReviewInfo(param);
            return true;
        }

        /// <summary>
        /// 删除用户战斗信息，删除用户信息时调用
        /// </summary>
        /// <param name="param">用户名</param>
        /// <returns>返回值，是否删除完成</returns>
        private static bool deleteReviewInfo(MySqlParameter param)
        {
            string deleteStr = "delete from battlereview where username = @username";
            MySqlHelper.ExecuteNonQuery(conn, CommandType.Text, deleteStr, param);
            return true;
        }
    }
}
