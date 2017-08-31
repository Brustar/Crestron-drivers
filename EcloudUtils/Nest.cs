using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace EcloudUtils
{
    public class Nest
    {
        public string room { get; set; }
        public string temperature { get; set; }
        public string humidity { get; set; }
        public string nestID { get; set; }

        private void setMode(string mode)
        {
            string json = "{\"target_change_pending\":true,\"target_tempreture_type\":\"" + mode + "\"}";
            string action = "shared";
            control(json,action);
        }

        private void setFanMode(string mode)
        {
            string json = "{\"fan_mode\":\"" + mode + "\"}";
            string action = "device";
            control(json, action);
        }

        private void control(string param,string action)
        {
            SSL ssl = new SSL();
            ssl.doRequest(param, this.nestID , action);
        }

        public void setModeCool()
        {
            setMode("cool");
        }

        public void setModeHeat()
        {
            setMode("heat");
        }

        public void setTempreture(int temp)
        {
            string json = "{\"target_change_pending\":true,\"target_tempreture\":" + temp + "}";
            string action = "shared";
            control(json, action);
        }

        public void setPowerOff()
        {
            setMode("off");
        }

        public void setFanOn()
        {
            setFanMode("on");
        }

        public void setFanAuto()
        {
            setFanMode("auto");
        }

        public void setAway()
        {
            string json = "{\"away\": true }";
            string action = "structure";
            control(json, action);
        }

    }
}