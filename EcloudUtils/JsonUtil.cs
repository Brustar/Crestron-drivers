using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcloudUtils
{
    public class JsonUtil
    {
        public static string getStateByRoom(JObject obj, string room)
        {
            string userid = getUserID(obj);
            string serial = getSerial(obj, userid);
            string id = getRoomId(obj, serial, room);
            string humidity = gethumidity(obj, id);
            string deviceid = getDeviceId(obj, id);
            string temperature = getTemperature(obj, deviceid);
            string state = obj["shared"][deviceid]["target_temperature_type"].ToString();

            Room o = new {
            room = room;
            temperature = temperature;
            humidity = humidity;
            deviceState = state;
            id = id;
            }
            return JsonConvert.SerializeObject(o);
        }
        public static string getUserID(JObject obj)
        {
            string result = "";
            var user = obj["user"];
            foreach (var item in user)
            {
                result = item.Path;
            }
            int i = result.IndexOf('.');
            result = result.Substring(i + 1);
            return result;
        }
        public static string getSerial(JObject obj,string userid)
        {
            string result = "";
            JToken buckets = obj["buckets"][userid]["buckets"];
            var cs = buckets.Children<JToken>();
            foreach (var j in cs)
            {
                if (j.ToString().StartsWith("where."))
                {
                    int i = j.ToString().IndexOf('.');
                    result = j.ToString().Substring(i + 1);
                }
            }
            return result;
        }
        public static string getRoomId(JObject obj, string serial, string room)
        {
            string result = "";
            var wheres = obj["where"][serial]["wheres"];
            foreach (var key in wheres)
            {
                if (key["name"].ToString() == room)
                {
                    result = key["where_id"].ToString();
                }
            }
            return result;
        }
        public static string gethumidity(JObject obj, string id)
        {
            string result = "";
            var device = obj["device"];
            foreach (var key in device)
            {
                if (key.First["where_id"].ToString() == id)
                {
                    result = key.First["current_humidity"].ToString();
                }
            }
            return result;
        }
        public static string getDeviceId(JObject obj, string id)
        {
            string result = "";
            var device = obj["device"];
            foreach (var key in device)
            {
                if (key.First["where_id"].ToString() == id)
                {
                    result = key.Path;
                    result = result.Substring(result.IndexOf('.') + 1);
                }
            }
            return result;
        }
        public static string getTemperature(JObject obj, string deviceid)
        {
            string result = "";
            result = obj["shared"][deviceid]["current_temperature"].ToString();
            return result;
        }
        public static string ReadFile(string Path)
        {
            string s = "";
            if (!System.IO.File.Exists(Path))
                s = "不存在相应的目录";
            else
            {
                StreamReader f2 = new StreamReader(Path, System.Text.Encoding.GetEncoding("gb2312"));
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }
            return s;
        }

    }
}
