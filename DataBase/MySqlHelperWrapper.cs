using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Dapper;

namespace DataBase
{
    class MySqlHelperWrapper : MySQLHelper
    {
        class realtimedata : RealTimeData
        { }
        class realtimedata_min : RealTimeData
        { }
        class realtimedata_hour : RealTimeData
        { }
        class realtimedata_day : RealTimeData
        { }
        class realtimedata_month : RealTimeData
        { }
        string[] tableName = new string[] {
            "realtimedata",
            "realtimedata_min",
            "realtimedata_hour",
            "realtimedata_day",
            "realtimedata_month"
        };

        public MySqlHelperWrapper(string server, string port, string user, string password, bool isStatistics = true) : base(server, port, user, password, isStatistics)
        {
            //调用父类构造函数
        }

        public new Param[] QueryParam(string name)
        {
            var param= (List<Param>)base.QueryParam(name);
            return param.ToArray();
        }

        public RealTimeData[] QueryData(int index, string condition, object param = null)
        {
            List<RealTimeData> result=null;
            //var assembly = Assembly.GetAssembly(typeof(realtimedata));
            //var classType = assembly.GetTypes().SingleOrDefault(t => t.Name == tableName[index]);
            //var obj = Activator.CreateInstance(classType);
            //var t = Convert.ChangeType(obj, classType);
            //base.QueryData<t.>(condition, param);
            switch (index)
            {
                case 0:
                    result = (List<RealTimeData>)base.QueryData<realtimedata>(condition, param);
                    break;
                case 1:
                    result = (List<RealTimeData>)base.QueryData<realtimedata_min>(condition, param);
                    break;
                case 2:
                    result = (List<RealTimeData>)base.QueryData<realtimedata_hour>(condition, param);
                    break;
                case 3:
                    result = (List<RealTimeData>)base.QueryData<realtimedata_day>(condition, param);
                    break;
                case 4:
                    result = (List<RealTimeData>)base.QueryData<realtimedata_month>(condition, param);
                    break;
                default:
                    break;
            }
            
            return result.ToArray();
        }
        public RealTimeData[] QueryData(int index, DateTime startTime, DateTime endTime)
        {
            List<RealTimeData> result = null;
            switch (index)
            {
                case 0:
                    result = (List<RealTimeData>)base.QueryData<realtimedata>(startTime, endTime);
                    break;
                case 1:
                    result = (List<RealTimeData>)base.QueryData<realtimedata_min>(startTime, endTime);
                    break;
                case 2:
                    result = (List<RealTimeData>)base.QueryData<realtimedata_hour>(startTime, endTime);
                    break;
                case 3:
                    result = (List<RealTimeData>)base.QueryData<realtimedata_day>(startTime, endTime);
                    break;
                case 4:
                    result = (List<RealTimeData>)base.QueryData<realtimedata_month>(startTime, endTime);
                    break;
                default:
                    break;
            }
            return result.ToArray();
        }
        public void InsertData(object data)
        {
            //base.InsertData(data);
        }
    }
}
