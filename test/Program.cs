using System;
using Dapper;
using DataBase;

namespace test
{
    [Table("realtimedata")]
    class realtimedata : RealTimeData
    {
    }
    class Program
    {
        static void Main(string[] args)
        {
            string server = "127.0.0.1";
            string port = "3306";
            string user = "root";
            string password = "123456";
            MySQLHelper<realtimedata> mySQLHelper = new MySQLHelper<realtimedata>(server,port,user,password);
            mySQLHelper.Connect();
            realtimedata myClass = new realtimedata() {Value = 999, DateTime = DateTime.Now};
            for (int i = 0; i < 10; i++)
            {
                mySQLHelper.InsertData(myClass);
            }
            
            //var data = mySQLHelper.QueryData("select * from test.realtimedata");
            //foreach (var item in data)
            //{
            //    Console.WriteLine(item.Value+" "+item.DateTime);
            //}
            //mySQLHelper.DeleteData("delete from test.realtimedata where id > 3");
            Console.ReadKey();
            mySQLHelper.Disconnect();
        }
    }
}
