using System;
using DataBase;

namespace test
{
    class MyClass : RealTimeData
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
            MySQLHelper<MyClass> mySQLHelper = new MySQLHelper<MyClass>(server,port,user,password);
            mySQLHelper.Connect(server, port, user, password);
            MyClass myClass = new MyClass();
            myClass.DateTime = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                mySQLHelper.InsertData(myClass);
            }
            
            var data = mySQLHelper.QueryData("select * from test.realtimedata");
            foreach (var item in data)
            {
                Console.WriteLine(item.Value+" "+item.DateTime);
            }
            mySQLHelper.DeleteData("delete from test.realtimedata where id > 3");
            Console.ReadKey();
            mySQLHelper.Disconnect();
        }
    }
}
