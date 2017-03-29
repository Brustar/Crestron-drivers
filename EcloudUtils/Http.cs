using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;
using Crestron.SimplSharp.CrestronIO;

namespace EcloudUtils
{
    public class Http
    {
        public delegate void ResHandler(SimplSharpString data);
        public ResHandler OnRes { set; get; }

        private void request(string url,string param,RequestType method)
        {
            if (param != "")
            {
                url = url + "?" + param;
            }
            HttpClient client = new HttpClient();
            HttpClientRequest request = new HttpClientRequest();
            request.KeepAlive = true;
            request.Url.Parse(url);
            request.RequestType = method;
            client.DispatchAsync(request, (hscrs, e) =>
            {
                try
                {
                    if (hscrs.Code >= 200 && hscrs.Code < 300)
                    {
                        // success
                        string s = hscrs.ContentString.ToString();
                        CrestronConsole.Print("Data: " + s + "\n");
                        if (OnRes != null)
                        {
                            OnRes(s);
                        }
                    }
                }
                catch (Exception f)
                {
                    CrestronConsole.PrintLine("Connection error:::" + f.ToString());
                }
            });
        }

        public void get(string url, string param)
        {
            this.request(url, param, RequestType.Get);
        }

        public void post(string url, string param)
        {
            this.request(url, param, RequestType.Post);
        }

        private void checkPath(String filename)
        {
            String dir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dir))
                Directory.Create(dir);
        }

        public uint download(String url, String filename)
        {
            uint sz;

            try
            {
                checkPath(filename);
                HttpClient client = new HttpClient();
                int result = client.FgetFile(url, filename);
                if (result != 0)
                {
                    CrestronConsole.PrintLine("Transfer failed - " + result.ToString());
                    return 0;
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine(e.Message);
                return 0;
            }

            CrestronConsole.PrintLine("Success.");

            FileInfo fi = new FileInfo(filename);
            sz = (uint)fi.Length;
            fi = null;

            return sz;
        }

        public void handleHttp(string json)
        {
        }
    }
}