using System;
using DataBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        static MySQLHelper mySQLHelper;
        [ClassInitialize]
        public static void Init(TestContext testContext)
        {
            string server = "127.0.0.1";
            string port = "3306";
            string user = "root";
            string password = "123456";
            mySQLHelper = new MySQLHelper(server, port, user, password);
        }

        [TestMethod,Priority(0)]
        public void ConnectTest()
        {
            //连接到数据库
            mySQLHelper.Connect();
        }

        [TestMethod]
        [Priority(1)]
        public void InsertData()
        {
            //构造需要存储的数据
            RealTimeData myClass = new RealTimeData() { CreatedTime = DateTime.Now };
            for (int i = 0; i < 100; i++)
            {
                myClass.CreatedTime = myClass.CreatedTime.AddSeconds(10);
                myClass.OriginValue = i;
                mySQLHelper.InsertData(myClass);
            }
        }

        [TestMethod,Priority(100)]
        public void DisConnectTest()
        {
            //断开数据库连接
            mySQLHelper.Disconnect();
        }
    }
}
