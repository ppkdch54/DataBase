using Dapper;
using System;
using System.Collections.Generic;
/// <summary>
/// 参数表管理用户参数
/// 管理实时数据表
/// 管理扩展的数据表
/// </summary>
namespace DataBase
{
    interface IDataBase
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
        /// <param name="name">参数名称</param>
        /// <returns>参数值</returns>
        IEnumerable<Para> GetParam(string name);

        /// <summary>
        /// 写入参数 
        /// </summary>
        /// <param name="para">参数</param>
        void SetParam(Para para);

        /// <summary>
        /// 写入数据或修改数据
        /// </summary>
        /// <param name="data">待写入的数据</param>
        /// <returns></returns>
        void InsertData<T>(T data);

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="queryCondition">查询条件</param>
        /// <returns>返回的数据集合</returns>
        IEnumerable<T> QueryData<T>(object queryCondition);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="delCondition">删除条件</param>
        /// <returns>受影响数据数量</returns>
        int DeleteData<T>(object delCondition);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="delCondition">删除条件,必须用where开头的字符串条件</param>
        /// <returns>受影响数据数量</returns>
        int DeleteData<T>(string delConditionWhere);
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
        public int SiteNumber { get; set; }
        public int SensorNumber { get; set; }
        public float OriginValue { get; set; }
        public float CalcValue { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
