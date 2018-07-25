using Dapper;
using System;
using System.Collections.Generic;

namespace DataBase
{
    interface IDataBase<T> where T : RealTimeData
    {
        /// <summary>
        /// 连接数据库
        /// </summary>
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
        Para GetParam(string name);

        /// <summary>
        /// 写入参数 
        /// </summary>
        /// <param name="key">参数名称</param>
        /// <param name="value">参数值</param>
        void SetParam(Para para);

        /// <summary>
        /// 写入数据或修改数据
        /// </summary>
        /// <returns></returns>
        void InsertData(T data);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="queryCondition">查询条件</param>
        /// <returns>数据</returns>
        IEnumerable<T> QueryData(object queryCondition);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="delCondition">删除条件</param>
        /// <returns>受影响数据数量</returns>
        int DeleteData(object delCondition);
    }

    public class Para
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int AffectZone { get; set; }
    }

    public class RealTimeData
    {
        [Key]
        public int Id { get; set; }
        public int Value { get; set; }
        public DateTime DateTime { get; set; }
    }
}
