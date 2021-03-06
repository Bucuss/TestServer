﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestServer
{
    public class CommandParam
    {
        #region Connect request param
        public const int LOGIN_REQUEST = 1;            //登陆请求
        public const int LOADING_REQUEST = 2;          //载入请求
        public const int UPDATE_REQUEST = 3;           //更新请求
        public const int MATCH_REQUEST = 4;            //匹配请求
        public const int BATTLE_REQUEST = 5;           //对战请求
        public const int FINSH_REQUEST = 6;            //完成对战
        public const int IS_ONLINE = 7;                //检测在线请求
        #endregion

        #region Connect reply param
        public const int LOGIN_SUCCESS_REPLY = 1;      //登陆成功回应
        public const int LOGIN_FAIL_REPLY = 2;         //登陆失败回应
        public const int LOADING_SUCCESS_REPLY = 3;    //载入成功回应
        public const int UPDATE_FAIL_REPLY = 4;        //更新失败回应
        public const int MATCH_SUCCESS_REPLY = 5;      //匹配成功回应
        public const int MATCH_FAIL_REPLY = 6;         //匹配失败回应
        public const int BATTLE_REPLY = 7;             //对战回应
        #endregion

        #region
        public const int LOGIN_RETURN = 1;              //登陆返回值
        public const int LOAD_RETURN = 2;               //载入返回值
        public const int MATCH_RETURN = 3;              //匹配返回值
        public const int BATTLE_RETURN = 4;             //指令返回值
        public const int UPDATE_RETURN = 5;             //更新返回值
        public const int FINSH_RETURN = 6;              //完成返回值
        #endregion
    }
}
