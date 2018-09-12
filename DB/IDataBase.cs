using Dapper;
using System;

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
        /// 查询参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns>参数值</returns>
        //IEnumerable<Param> QueryParam(string name);

        /// <summary>
        /// 写入参数 
        /// </summary>
        /// <param name="para">参数</param>
        void InsertParam(Param para);

        /// <summary>
        /// 写入数据或修改数据
        /// </summary>
        /// <param name="data">待写入的数据</param>
        /// <returns></returns>
        void InsertData<T>(T data) where T : RealTimeData;

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="queryCondition">查询条件</param>
        /// <returns>返回的数据集合</returns>
        //IEnumerable<T> QueryData<T>(object queryCondition);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="delCondition">删除条件</param>
        /// <returns>受影响数据数量</returns>
        int DeleteData<T>(object delCondition) where T : RealTimeData;

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="delCondition">删除条件,必须用where开头的字符串条件</param>
        /// <returns>受影响数据数量</returns>
        //int DeleteData<T>(string delConditionWhere);
    }
    /// <summary>
    /// 参数表数据结构
    /// </summary>
    public class Param
    {
        /// <summary>
        /// 数据库主键
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }
    }
    /// <summary>
    /// 实时表数据结构
    /// </summary>
    public class RealTimeData
    {
        /// <summary>
        /// 数据库主键
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 站号
        /// </summary>
        public int? SiteNumber { get; set; }
        /// <summary>
        /// 传感器编号
        /// </summary>
        public int? SensorNumber { get; set; }
        /// <summary>
        /// 数据原始值
        /// </summary>
        public float? OriginValue { get; set; }
        /// <summary>
        /// 计算值
        /// </summary>
        public float? CalcValue { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdatedTime { get; set; }
    }
}
