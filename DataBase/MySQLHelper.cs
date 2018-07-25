using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Dapper;

namespace DataBase
{
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
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
        }

        public bool Connect()
        {
            mysqlConnection.Open();
            mysqlConnection.ChangeDatabase("test");
            return true;
        }

        public void Disconnect()
        {
            mysqlConnection.Close();
        }

        public void SetParam(Para para)
        {
            mysqlConnection.Insert(para);
        }

        public Para GetParam(string name)
        {
            return mysqlConnection.Get<Para>(new Para { Name = name });
        }

        public void InsertData(T t)
        {
            mysqlConnection.Insert(t);
        }

        public IEnumerable<T> QueryData(object condition = null)
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

        public IEnumerable<T> QueryData(string condition)
        {
            var result = mysqlConnection.GetList<T>(condition);
            return result;
        }

        public int DeleteData(object delCondition)
        {
            return mysqlConnection.DeleteList<T>(delCondition);
        }

        public int DeleteData(string delCondition)
        {
            return mysqlConnection.DeleteList<T>(delCondition);
        }

    }
}
