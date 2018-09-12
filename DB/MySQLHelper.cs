using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Dapper;
using System;

namespace DataBase
{
    /// <summary>
    /// 数据库模块
    /// </summary>
    public class MySQLHelper : IDataBase
    {
        MySqlConnection mysqlConnection;
        string connStr;
        /// <summary>
        /// 是否需要对原始数据进行统计
        /// </summary>
        bool isStatistics;
        /// <summary>
        /// 配置数据库连接参数
        /// </summary>
        /// <param name="server">数据库服务器地址</param>
        /// <param name="port">数据库服务器端口</param>
        /// <param name="user">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="isStatistics">是否需要统计实时数据</param>
        public MySQLHelper(string server, string port, string user, string password, bool isStatistics = true)
        {
            connStr = "datasource=" + server
                + ";port=" + port
                + ";user=" + user
                + ";pwd=" + password + ";SslMode = none;";
            mysqlConnection = new MySqlConnection(connStr);
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
            this.isStatistics = isStatistics;
        }
        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns>连接是否成功</returns>
        public bool Connect()
        {
            try
            {
                mysqlConnection.Open();
                string cmdStr = "SELECT * FROM information_schema.SCHEMATA where SCHEMA_NAME='sxddck_db'; ";
                System.Data.IDataReader result = mysqlConnection.ExecuteReader(cmdStr);
                bool isExist = result.Read();
                result.Close();
                if (!isExist)
                {
                    //创建数据库
                    mysqlConnection.Execute(@"CREATE DATABASE `sxddck_db`;");
                    mysqlConnection.ChangeDatabase("sxddck_db");
                    //创建6张表
                    string[] tableNames = isStatistics ? new string[] { "", "_month", "_day", "_hour", "_min" } : new string[] { "" };
                    foreach (var name in tableNames)
                    {
                        CreateTable(name);
                    }
                    //创建参数表
                    CreateParaTable();
                    if (isStatistics)
                    {
                        //创建统计触发器
                        CreateTriggers();
                    }
                }
                else
                {
                    mysqlConnection.ChangeDatabase("sxddck_db");
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return true;
        }
        /// <summary>
        /// 断开数据库连接
        /// </summary>
        public void Disconnect()
        {
            mysqlConnection.Close();
        }
        /// <summary>
        /// 存入参数
        /// </summary>
        /// <param name="param">参数值</param>
        public void InsertParam(Param param)
        {
            using (mysqlConnection = GetOpenConnection())
            {
                mysqlConnection.Insert(param);
            }
        }
        /// <summary>
        /// 查询参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns>返回参数集合</returns>
        public IEnumerable<Param> QueryParam(string name)
        {
            IEnumerable<Param> retVals = null;
            using (mysqlConnection = GetOpenConnection())
            {
                retVals = mysqlConnection.GetList<Param>(new { Name = name });
            }
            return retVals;
        }
        /// <summary>
        /// 依据参数名删除参数表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int DeleteParam(string name)
        {
            int retVal = 0;
            using (mysqlConnection = GetOpenConnection())
            {
                retVal= mysqlConnection.DeleteList<Param>(new { Name = name });
            }
            return retVal;
        }
        /// <summary>
        /// 删除所有参数
        /// </summary>
        /// <returns></returns>
        public int DeleteAllParams()
        {
            int retVal = 0;
            using (mysqlConnection = GetOpenConnection())
            {
                retVal = mysqlConnection.DeleteList<Param>("where Id <> -1");
            }
            return retVal;
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">数据库映射类</typeparam>
        /// <param name="data">数据</param>
        public void InsertData<T>(T data) where T:RealTimeData
        {
            using (mysqlConnection = GetOpenConnection())
            {
                mysqlConnection.Insert(data);
            }
        }
        /// <summary>
        /// 查询数据,该接口暂时不开放
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        private IEnumerable<T> QueryData<T>(object condition = null) where T : RealTimeData
        {
            if (condition != null)
            {
                var result = mysqlConnection.GetList<T>(condition);
                return result;
            }
            else
            {
                var result = mysqlConnection.GetList<T>();
                return result;
            }
        }
        /// <summary>
        /// 特殊数据查询接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryData<T>(string condition, object param = null) where T : RealTimeData
        {
            IEnumerable<T> result = null;
            using (mysqlConnection = GetOpenConnection())
            {
                result = mysqlConnection.GetList<T>(condition, param);
            }
            return result;
        }
        /// <summary>
        /// 用开始和结束时间进行查询的接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startTime">查询起始时间</param>
        /// <param name="endTime">查询结束时间</param>
        /// <returns>返回可枚举的记录集合</returns>
        public IEnumerable<T> QueryData<T>(DateTime startTime, DateTime endTime) where T : RealTimeData
        {
            IEnumerable<T> result = null;
            using (mysqlConnection = GetOpenConnection())
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("startTime", startTime);
                dynamicParameters.Add("endTime", endTime);
                result = QueryData<T>(@"where CreatedTime >= @startTime and CreatedTime <= @endTime", dynamicParameters);
            }
            return result;
        }
        /// <summary>
        /// 依据单条件查询
        /// </summary>
        /// <typeparam name="T">查询表名称</typeparam>
        /// <param name="delCondition">查询条件</param>
        /// <returns>受影响的条目数</returns>
        public int DeleteData<T>(object delCondition = null) where T : RealTimeData
        {
            int retVal = 0;
            using (mysqlConnection = GetOpenConnection())
            {
                retVal = mysqlConnection.DeleteList<T>(delCondition);
            }
            return retVal;
        }
        /// <summary>
        /// where语句查询
        /// </summary>
        /// <typeparam name="T">查询表名称</typeparam>
        /// <param name="delCondition">查询条件</param>
        /// <returns>受影响的条目数</returns>
        private int DeleteData<T>(string delCondition) where T : RealTimeData
        {
            int retVal = 0;
            using (mysqlConnection = GetOpenConnection())
            {
                retVal = mysqlConnection.DeleteList<T>(delCondition);
            }
            return retVal;
        }
        private void CreateTriggers()
        {
            mysqlConnection.Execute(@"
                DROP TRIGGER IF EXISTS `tri_realTimeMinDataInsert`;
                create trigger tri_realTimeMinDataInsert after insert
                on realtimedata for each row
                begin
                DECLARE curr DATETIME;
                DECLARE curr_min DATETIME;
                DECLARE last_min_sum DOUBLE;
                DECLARE is_exists_min INT DEFAULT 0;

                SET curr = new.CreatedTime;
                set curr_min = SUBDATE(curr,INTERVAL SECOND(curr) SECOND);
                set last_min_sum = convert((select max(OriginValue) from realtimedata_min where SiteNumber =new.SiteNumber and SensorNumber = new.SensorNumber and CreatedTime = curr_min),decimal(10,3));

                SELECT COUNT(*) INTO is_exists_min FROM realtimedata_min WHERE SiteNumber = new.SiteNumber AND 
                SensorNumber=new.SensorNumber AND CreatedTime = curr_min;

                IF is_exists_min>0 THEN
                UPDATE realtimedata_min SET UpdatedTime = curr, `Count` = `Count`+1,OriginValue = 
                new.OriginValue +last_min_sum, CalcValue = (new.OriginValue+last_min_sum)/`Count`
                WHERE SiteNumber = new.SiteNumber AND SensorNumber=new.SensorNumber AND CreatedTime = curr_min;
                ELSE 
                INSERT INTO realtimedata_min
                (SiteNumber,SensorNumber,OriginValue,CalcValue,`Count`,CreatedTime,UpdatedTime)
                VALUES(new.SiteNumber,new.SensorNumber,new.OriginValue,new.OriginValue,1,curr_min,curr);
                END IF ;

                end");
            mysqlConnection.Execute(@"
                DROP TRIGGER IF EXISTS `tri_realTimeHourDataInsert`; 
                create trigger tri_realTimeHourDataInsert after insert
                on realtimedata for each row
                begin
                DECLARE curr DATETIME;
                DECLARE curr_min DATETIME;
                DECLARE curr_hour DATETIME;
                DECLARE last_hour_sum DOUBLE;
                DECLARE is_exists_hour INT DEFAULT 0;

                SET curr = new.CreatedTime;
                set curr_min = SUBDATE(curr,INTERVAL SECOND(curr) SECOND);
                set curr_hour = SUBDATE(curr_min,INTERVAL MINUTE(curr_min) MINUTE);
                set last_hour_sum = convert((select max(OriginValue) from realtimedata_hour where SiteNumber =new.SiteNumber and SensorNumber = new.SensorNumber and CreatedTime = curr_hour),decimal(10,3));

                SELECT COUNT(*) INTO is_exists_hour FROM realtimedata_hour WHERE SiteNumber = new.SiteNumber AND 
                SensorNumber=new.SensorNumber AND CreatedTime = curr_hour;

                IF is_exists_hour>0 THEN
                UPDATE realtimedata_hour SET UpdatedTime = curr, `Count` = `Count`+1,OriginValue = 
                new.OriginValue +last_hour_sum, CalcValue = (new.OriginValue+last_hour_sum)/`Count`
                WHERE SiteNumber = new.SiteNumber AND SensorNumber=new.SensorNumber AND CreatedTime = curr_hour;
                ELSE 
                INSERT INTO realtimedata_hour
                (SiteNumber,SensorNumber,OriginValue,CalcValue,`Count`,CreatedTime,UpdatedTime)
                VALUES(new.SiteNumber,new.SensorNumber,new.OriginValue,new.OriginValue,1,curr_hour,curr);
                END IF ;

                end
                ");
            mysqlConnection.Execute(@"
                DROP TRIGGER IF EXISTS `tri_realTimeDayDataInsert`; 
                create trigger tri_realTimeDayDataInsert after insert
                on realtimedata for each row
                begin
                DECLARE curr DATETIME;
                DECLARE curr_min DATETIME;
                DECLARE curr_hour DATETIME;
                DECLARE curr_day DATETIME;
                DECLARE last_day_sum DOUBLE;
                DECLARE is_exists_day INT DEFAULT 0;

                SET curr = new.CreatedTime;
                set curr_min = SUBDATE(curr,INTERVAL SECOND(curr) SECOND);
                set curr_hour = SUBDATE(curr_min,INTERVAL MINUTE(curr_min) MINUTE);
                set curr_day = SUBDATE(curr_hour,INTERVAL HOUR(curr_hour) HOUR);
                set last_day_sum = convert((select max(OriginValue) from realtimedata_day where SiteNumber =new.SiteNumber and SensorNumber = new.SensorNumber and CreatedTime = curr_day),decimal(10,3));

                SELECT COUNT(*) INTO is_exists_day FROM realtimedata_day WHERE SiteNumber = new.SiteNumber AND 
                SensorNumber=new.SensorNumber AND CreatedTime = curr_day;

                IF is_exists_day>0 THEN
                UPDATE realtimedata_day SET UpdatedTime = curr, `Count` = `Count`+1,OriginValue = 
                new.OriginValue +last_day_sum, CalcValue = (new.OriginValue+last_day_sum)/`Count`
                WHERE SiteNumber = new.SiteNumber AND SensorNumber=new.SensorNumber AND CreatedTime = curr_day;
                ELSE 
                INSERT INTO realtimedata_day
                (SiteNumber,SensorNumber,OriginValue,CalcValue,`Count`,CreatedTime,UpdatedTime)
                VALUES(new.SiteNumber,new.SensorNumber,new.OriginValue,new.OriginValue,1,curr_day,curr);
                END IF ;

                end");
            mysqlConnection.Execute(@"
                DROP TRIGGER IF EXISTS `tri_realTimeMonthDataInsert`; 
                create trigger tri_realTimeMonthDataInsert after insert
                on realtimedata for each row
                begin
                DECLARE curr DATETIME;
                DECLARE curr_min DATETIME;
                DECLARE curr_hour DATETIME;
                DECLARE curr_day DATETIME;
                DECLARE curr_month DATETIME;
                DECLARE last_month_sum DOUBLE;
                DECLARE is_exists_month INT DEFAULT 0;

                SET curr = new.CreatedTime;
                set curr_min = SUBDATE(curr,INTERVAL SECOND(curr) SECOND);
                set curr_hour = SUBDATE(curr_min,INTERVAL MINUTE(curr_min) MINUTE);
                set curr_day = SUBDATE(curr_hour,INTERVAL HOUR(curr_hour) HOUR);
                set curr_month = SUBDATE(curr_day,INTERVAL DAY(curr_day)-1 DAY);
                set last_month_sum = convert((select max(OriginValue) from realtimedata_month where SiteNumber =new.SiteNumber and SensorNumber = new.SensorNumber and CreatedTime = curr_month),decimal(10,3));

                SELECT COUNT(*) INTO is_exists_month FROM realtimedata_month WHERE SiteNumber = new.SiteNumber AND 
                SensorNumber=new.SensorNumber AND CreatedTime = curr_month;

                IF is_exists_month>0 THEN
                UPDATE realtimedata_month SET UpdatedTime = curr, `Count` = `Count`+1,OriginValue = 
                new.OriginValue +last_month_sum, CalcValue = (new.OriginValue+last_month_sum)/`Count`
                WHERE SiteNumber = new.SiteNumber AND SensorNumber=new.SensorNumber AND CreatedTime = curr_month;
                ELSE 
                INSERT INTO realtimedata_month
                (SiteNumber,SensorNumber,OriginValue,CalcValue,`Count`,CreatedTime,UpdatedTime)
                VALUES(new.SiteNumber,new.SensorNumber,new.OriginValue,new.OriginValue,1,curr_month,curr);
                END IF ;

                end
            ");
        }
        private void CreateParaTable()
        {
            mysqlConnection.Execute(
                @"CREATE TABLE `param` (
                `Id` int(11) NOT NULL AUTO_INCREMENT,
                `Name` varchar(45) DEFAULT NULL,
                `Value` varchar(45) DEFAULT NULL,
                PRIMARY KEY (`Id`)
                ) ENGINE=InnoDB AUTO_INCREMENT=0 DEFAULT CHARSET=utf8"
            );
        }
        private void CreateTable(string tableName)
        {
            //创建表
            mysqlConnection.Execute(
                @"CREATE TABLE `realtimedata" + tableName + @"` (
                `Id` int(11) NOT NULL AUTO_INCREMENT,
                `SiteNumber` int(11) DEFAULT NULL,
                `SensorNumber` int(11) DEFAULT NULL,
                `OriginValue` float DEFAULT NULL,
                `CalcValue` float DEFAULT NULL,
                `Count` int(11) DEFAULT NULL,
                `CreatedTime` datetime DEFAULT NULL,
                `UpdatedTime` datetime DEFAULT NULL,
                PRIMARY KEY(`Id`)
                ) ENGINE = InnoDB AUTO_INCREMENT = 0 DEFAULT CHARSET=utf8"
            );
        }
        private MySqlConnection GetOpenConnection()
        {
            MySqlConnection mysqlConnection = new MySqlConnection(connStr);
            mysqlConnection.Open();
            mysqlConnection.ChangeDatabase("sxddck_db");
            return mysqlConnection;
        }
    }

    /// <summary>
    /// 原始数据表
    /// </summary>
    public class realtimedata : RealTimeData { }
    /// <summary>
    /// 按分钟统计
    /// </summary>
    public class realtimedata_min : RealTimeData { }
    /// <summary>
    /// 按小时统计
    /// </summary>
    public class realtimedata_hour : RealTimeData { }
    /// <summary>
    /// 按天统计
    /// </summary>
    public class realtimedata_day : RealTimeData { }
    /// <summary>
    /// 按月统计
    /// </summary>
    public class realtimedata_month : RealTimeData { }
}
