using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
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
        //public int Id { get; set; }
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
        }

        public bool Connect()
        {
            mysqlConnection.Open();
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
            var propInfos = t.GetType().GetProperties();
            List<string> columns = new List<string>();
            for (int k = 0; k < 4; k++)
            {
                columns.Add("realtimedatacol" + (k + 1));
            }
            string sql = @"INSERT INTO `test`.`realtimedata` values (?Id)";// (";

            MySqlCommand command = new MySqlCommand(sql, mysqlConnection);
            command.CommandText = sql;
            command.Parameters.AddWithValue("?Id",1);
            sql += "VALUES (";
            for (int i = 1; i < columns.Count; i++)
            {
                if (i>=propInfos.Length)
                {
                    command.Parameters.Add(new MySqlParameter(columns[i], DBNull.Value));
                    continue;
                }
                var prop = propInfos[i];
                object value = prop.GetValue(t);
                if (value == null)
                {
                    command.Parameters.Add(new MySqlParameter(columns[i], DBNull.Value));
                }
                else
                {
                    command.Parameters.Add(new MySqlParameter(columns[i], value));
                }
            }
            //mysqlConnection.Execute(command.CommandText);
            command.ExecuteNonQuery();
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
