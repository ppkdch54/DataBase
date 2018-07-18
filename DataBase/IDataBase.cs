using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace DataBase
{
    interface IDataBase<T> where T:RealTimeData
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="url">数据库地址</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>是否成功</returns>
        bool Connect();

        /// <summary>
        /// 断开连接
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 读取参数
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <returns>参数值</returns>
        string GetParam(string key);

        /// <summary>
        /// 写入参数 
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        void SetParam(string key, string value);

        /// <summary>
        /// 写入数据或修改数据
        /// </summary>
        /// <param name="key">数据名称</param>
        /// <param name="value">数据值</param>
        /// <param name="time">日期</param>
        /// <returns></returns>
        void InsertData(T t);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="queryCondition">查询条件</param>
        /// <returns>数据</returns>
        IEnumerable<T> QueryData(string queryCondition);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="delCondition">删除条件</param>
        /// <returns>受影响数据数量</returns>
        int DeleteData(string delCondition);
    }
}
