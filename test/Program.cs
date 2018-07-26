using System;
using Dapper;
using DataBase;

namespace test
{
    [Table("realtimedata")]
    class realtimedata : RealTimeData
    {
        public int MyProperty { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string server = "127.0.0.1";
            string port = "3306";
            string user = "root";
            string password = "123456";
            MySQLHelper<realtimedata> mySQLHelper = new MySQLHelper<realtimedata>(server, port, user, password);
            mySQLHelper.Connect();
            realtimedata myClass = new realtimedata() { Value = 9, DateTime = DateTime.Now, MyProperty = 99 };
            for (int i = 0; i < 10; i++)
            {
                mySQLHelper.InsertData(myClass);
            }
            mySQLHelper.DeleteData(new { value = 99 });
            var data = mySQLHelper.QueryData();
            foreach (var item in data)
            {
                Console.WriteLine(item.Id + ": " + item.Value + ", " + item.DateTime);
            }
            Para para = new Para() { Name = "标题", Value = "戴德测控", Type = "string", AffectZone = 0 };
            mySQLHelper.SetParam(para);
            Console.WriteLine("Success!");
            Console.ReadKey();
            mySQLHelper.Disconnect();
        }
    }
}
