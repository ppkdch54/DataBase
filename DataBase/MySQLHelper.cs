using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Dapper;
using System;

namespace DataBase
{
    public class MySQLHelper : IDataBase
    {
        MySqlConnection mysqlConnection;
        public MySQLHelper(string server, string port, string user, string password)
        {
            string connStr = "datasource=" + server
                + ";port=" + port
                + ";user=" + user
                + ";pwd=" + password + ";";
            mysqlConnection = new MySqlConnection(connStr);
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
        }

        public bool Connect()
        {
            mysqlConnection.Open();
            string cmdStr = "SELECT * FROM sys.schema_table_statistics where TABLE_SCHEMA='test'; ";
            System.Data.IDataReader result = mysqlConnection.ExecuteReader(cmdStr);
            bool isExist = result.Read();
            result.Close();
            if (!isExist)
            {
                //创建数据库
                mysqlConnection.Execute(@"CREATE DATABASE `test` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */;");
                mysqlConnection.ChangeDatabase("test");
                //创建6张表
                string[] tableNames = new string[] { "", "_month", "_day", "_hour", "_min" };
                foreach (var name in tableNames)
                {
                    CreateTable(name);
                }
                //创建参数表
                CreateParaTable();
                //创建统计触发器
                CreateTriggers();
            }
            else
            {
                mysqlConnection.ChangeDatabase("test");
            }
            return true;
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

        public void Disconnect()
        {
            mysqlConnection.Close();
        }
        public void CreateParaTable()
        {
            mysqlConnection.Execute(
                @"CREATE TABLE `para` (
                `Id` int(11) NOT NULL AUTO_INCREMENT,
                `Name` varchar(45) DEFAULT NULL,
                `Type` varchar(45) DEFAULT NULL,
                `Value` varchar(45) DEFAULT NULL,
                `AffectZone` int(11) DEFAULT NULL,
                PRIMARY KEY (`Id`)
                ) ENGINE=InnoDB AUTO_INCREMENT=0 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci"
            );
        }
        public void CreateTable(string tableName)
        {
            //创建表
            mysqlConnection.Execute(
                @"CREATE TABLE `realtimedata"+tableName+ @"` (
                `Id` int(11) NOT NULL AUTO_INCREMENT,
                `SiteNumber` int(11) DEFAULT NULL,
                `SensorNumber` int(11) DEFAULT NULL,
                `OriginValue` float DEFAULT NULL,
                `CalcValue` float DEFAULT NULL,
                `Count` int(11) DEFAULT NULL,
                `CreatedTime` datetime DEFAULT NULL,
                `UpdatedTime` datetime DEFAULT NULL,
                PRIMARY KEY(`Id`)
                ) ENGINE = InnoDB AUTO_INCREMENT = 0 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci"
            );
        }

        public void SetParam(Para para)
        {
            mysqlConnection.Insert(para);
        }

        public Para GetParam(string name)
        {
            return mysqlConnection.Get<Para>(new Para { Name = name });
        }

        public void InsertData<T>(T t)
        {
            mysqlConnection.Insert(t);
        }

        public IEnumerable<T> QueryData<T>(object condition = null)
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

        public IEnumerable<T> QueryData<T>(string condition)
        {
            var result = mysqlConnection.GetList<T>(condition);
            return result;
        }

        public int DeleteData<T>(object delCondition=null)
        {
            return mysqlConnection.DeleteList<T>(delCondition);
        }

        public int DeleteData<T>(string delCondition)
        {
            return mysqlConnection.DeleteList<T>(delCondition);
        }

    }
}
