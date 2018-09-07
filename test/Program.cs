using System;
using Dapper;
using DataBase;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            //连接到数据库
            string server = "127.0.0.1";
            string port = "3306";
            string user = "root";
            string password = "123456";
            MySQLHelper mySQLHelper = new MySQLHelper(server, port, user, password,false);
            if (!mySQLHelper.Connect())
            {
                Console.WriteLine("Connect fail!");
                return;
            }
            else
            {
                Console.WriteLine("Connect success!");
            }
            mySQLHelper.DeleteAllParams();
            var r = mySQLHelper.QueryData<realtimedata>(DateTime.Parse("2018-08-01 09:24:57"), DateTime.Parse("2018-08-01 09:25:47"));
            foreach (var item in r)
            {
                Console.WriteLine(item.Id + ": " + item.SiteNumber + "," + item.OriginValue + ", " + item.CreatedTime);
            }
            mySQLHelper.DeleteData<realtimedata>();
            //构造需要存储的数据条目
            realtimedata myClass = new realtimedata() { OriginValue = 9, CreatedTime = DateTime.Now };
            for (int i = 0; i < 100; i++)
            {
                myClass.SiteNumber = 0;
                myClass.SensorNumber = 0;
                myClass.CreatedTime = myClass.CreatedTime.Value.AddSeconds(10);
                myClass.OriginValue = i;
                mySQLHelper.InsertData(myClass);
            }
            //构造删除数据的条件,并删除符合条件的数据库内容
            //mySQLHelper.DeleteData<realtimedata>(new { OriginValue = 99 });
            //查询所有实时数据,并显示
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("CreatedTime", DateTime.Now.AddSeconds(60));
            var result = mySQLHelper.QueryData<realtimedata>("where CreatedTime < @CreatedTime", dynamicParameters);

            Console.WriteLine("realtime!");
            foreach (var item in result)
            {
                Console.WriteLine(item.Id + ": " + item.SiteNumber + "," + item.OriginValue + ", " + item.CreatedTime);
            }
            Console.WriteLine("QueryData success!");
            ////查询所有分钟数据,并显示
            //var data_min = mySQLHelper.QueryData<realtimedata_min>("where id > 30");
            //Console.WriteLine("realtime_min!");
            //foreach (var item in data_min)
            //{
            //    Console.WriteLine(item.Id + ": " + item.OriginValue + ", " + item.CreatedTime);
            //}
            //构造参数条目,并存储
            Param para = new Param() { Name = "标题", Value = "戴德测控" };
            mySQLHelper.InsertParam(para);
            var pr = mySQLHelper.QueryParam("标题");
            foreach (var item in pr)
            {
                Console.WriteLine("para:" + item.Value);
            }
            Console.WriteLine("QueryParam Success!");
            //断开数据库连接
            mySQLHelper.Disconnect();
            //示例程序结束标志
            Console.WriteLine("Disconnect Success!");
            Console.ReadKey();
        }
    }
}
