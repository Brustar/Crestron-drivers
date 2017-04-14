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
            //string state = obj["shared"][deviceid]["target_temperature_type"].ToString();
            /*
            var o = new {
            room = room;
            temperature = temperature;
            humidity = humidity;
            deviceState = state;
            id = id;
            }
            return JsonConvert.SerializeObject(o);
            */
            return temperature + "," + humidity;
        }
        
        public static string getUserID(JObject obj)
        {
            string result = "";
            JObject user = (JObject)obj["user"];
            foreach (var item in user)
            {
                result = item.Key;
            }
            return result;
        }
        
        public static string getSerial(JObject obj,string userid)
        {
            string result = "";
            JToken buckets = (JToken)obj["buckets"][userid]["buckets"];
            var cs = buckets.Children();
            foreach (var item in cs)
            {
                string j = trimQuot(item.ToString());
                if (j.StartsWith("where."))
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
                string temp = trimQuot(key["name"].ToString());
                if (temp == room)
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
                string temp = trimQuot(key.First["where_id"].ToString());
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
            JObject device = (JObject)obj["device"];
            foreach (var item in device)
            {
                string temp = trimQuot(item.Value["where_id"].ToString());
                if (item.Value["where_id"].ToString() == id)
                {
                    result = item.Key;
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

        public string parseURL(string json)
        {
            string ret = "";
            var obj = JObject.Parse(json.ToString());
            ret = obj["plist_url"].ToString();
            return ret;
        }

        public static string trimQuot(string data)
        {
            if (data.StartsWith("\"") && data.EndsWith("\""))
            {
                data = data.Substring(1, data.Length - 2);
            }
            return data;
        }
        
    }
}
