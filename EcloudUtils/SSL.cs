using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Https;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EcloudUtils
{
    public class SSL
    {
        public delegate void ResHandler(SimplSharpString data);
        public ResHandler OnRes { set; get; }

        public void doPost(string username,string password)
        {
            string userdefault = Txt.read(Txt.path);
            if (userdefault != "")
            {
                JObject obj = JObject.Parse(userdefault);

                string expire = obj["expires_in"].ToString();
                DateTime exp = DateTime.Parse(expire);
                if (exp.CompareTo(DateTime.Now) < 0)
                {
                    handleLogin(userdefault);
                    return;
                }   
            }

            string url = "https://home.nest.com/user/login";
            url = url + "?username=" + username + "&password=" + password;
            HttpsClient client = new HttpsClient();
            client.Verbose = false;
            client.PeerVerification = false;
            client.HostVerification = false;
            HttpsClientRequest request = new HttpsClientRequest();
            request.KeepAlive = true;
            request.Url.Parse(url);
            request.RequestType = RequestType.Post;

            client.DispatchAsync(request, (hscrs, e) =>
            {
                try
                {
                    if (hscrs.Code >= 200 && hscrs.Code < 300)
                    {
                        // success
                        string s = hscrs.ContentString.ToString();
                        CrestronConsole.Print("Https Data: " + s + "\n");
                        handleLogin(s);
                    }
                }
                catch (Exception f)
                {
                    CrestronConsole.PrintLine("Connection error:" + f.ToString());
                }
            });
            
        }

        public void doGet(string url, Hashtable header)
        {
            HttpsClient client = new HttpsClient();
            client.Verbose = false;
            client.PeerVerification = false;
            client.HostVerification = false;
            HttpsClientRequest request = new HttpsClientRequest();
            request.KeepAlive = true;
            request.Url.Parse(url);
            request.RequestType = RequestType.Get;
            if (header != null && header.Count > 0)
            {
                foreach (DictionaryEntry a in header)
                {
                    request.Header.SetHeaderValue(a.Key.ToString(), a.Value.ToString());
                }
            }
            client.DispatchAsync(request, (hscrs, e) =>
            {
                try
                {
                    if (hscrs.Code >= 200 && hscrs.Code < 300)
                    {
                        // success
                        string s = hscrs.ContentString.ToString();
                        CrestronConsole.Print("Https Data: " + s + "\n");
                        if (OnRes != null)
                        {
                            OnRes(s);
                        }
                    }
                }
                catch (Exception f)
                {
                    CrestronConsole.PrintLine("Connection error:" + f.ToString());
                }
            });
        }

        public void handleLogin(string json)
        {
            JObject obj = JObject.Parse(json);
            string token = obj["access_token"].ToString();
            string user = obj["user"].ToString();
            string user_id = obj["userid"].ToString();
            string expire = obj["expires_in"].ToString();
            DateTime exp = DateTime.Parse(expire);
            if (exp.CompareTo(DateTime.Now) >= 0)
            {
                Txt.write(Txt.path,json);
            }
            string url = obj["urls"]["transport_url"].ToString();
            url = url + "/v3/mobile/" + user;
            Hashtable ht = new Hashtable();
            ht.Add("X-nl-protocol-version", "1");
            ht.Add("X-nl-user-id", user_id);
            ht.Add("Authorization", "Basic " + token);
            doGet(url, ht);
        }

        public string[] handleNestStatus(string json,string room)
        {
            string[] temp = new string[2];
            var obj = JObject.Parse(json);
            var nestuct = obj["structure"];
            var devices = nestuct["devices"];
            foreach(var o in devices)
            {
                string deviceID = o.ToString().Substring(7);
                string temperature = obj["shared"][deviceID]["current_temperature"].ToString();
                string humidity = obj["shared"][deviceID]["current_humidity"].ToString();
                string where = obj["shared"][deviceID]["where"].ToString();
                if (where == room)
                {
                    temp[0] = temperature;
                    temp[1] = humidity;
                }
            }
            return temp;
        }

    }
}