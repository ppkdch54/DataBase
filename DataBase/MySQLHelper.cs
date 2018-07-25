using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Dapper;

namespace DataBase
{
    public class Para
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class RealTimeData
    {
        [Key]
        public int Id { get; set; }
        public int Value { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class MySQLHelper<T> : IDataBase<T> where T : RealTimeData
    {
        MySqlConnection mysqlConnection;
        public MySQLHelper(string server, string port, string user, string password)
        {
            string connStr = "datasource=" + server
                + ";port=" + port
                + ";user=" + user
                + ";pwd=" + password + ";";
            mysqlConnection = new MySqlConnection(connStr);
            Dapper.SimpleCRUD.SetDialect(Dapper.SimpleCRUD.Dialect.MySQL);
        }

        public bool Connect()
        {
            mysqlConnection.Open();
            mysqlConnection.ChangeDatabase("test");
            return true;
        }

        public void Disconnect()
        {
            Close();
        }

        public string GetParam(string key)
        {
            throw new NotImplementedException();
        }

        public void InsertData(T t) 
        {
            mysqlConnection.Insert(t);
        }

        public IEnumerable<T> QueryData(string queryCondition)
        {
            var result = mysqlConnection.Query<T>(queryCondition);
            return result;
        }

        public void SetParam(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            mysqlConnection.Close();
        }

        public int DeleteData(string delCondition)
        {
            return mysqlConnection.Execute(delCondition);
        }
    }
}
