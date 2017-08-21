using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Scheduler;
using System.Collections;

namespace EcloudUtils
{
    public enum schType
    {
        device,
        scene
    }

    public class Schedule
    {
        public string startTime;
        public string endTime;
        public int interval;
        public ArrayList weekDays;
    }

    public class Scheduler
    {
        private int schdulerID;
        private int hostID;
        private schType schduleType;
        private ScheduledEventGroup schGroup;
        public delegate void Handler(SimplSharpString data);
        public Handler openit { set; get; }
        public Handler closeit { set; get; }

        public Scheduler()
        {
            if (this.schGroup == null)
            {
                this.schGroup = new ScheduledEventGroup("ecloud");
            }
        }

        public void prepare(int host,int id,schType type)
        {
            this.schdulerID = id;
            this.hostID = host;
            this.schduleType = type;
            Http http = new Http();
            string fileName = "\\NVRAM\\" + host + "_" + id + ".plist";
            http.OnRes += response;

            http.prepareDownload(fileName);
        }

        private void response(SimplSharpString data)
        {
            string fileName = "\\NVRAM\\" + this.hostID + "_" + this.schdulerID + ".plist";
            string url = new JsonUtil().parseURL(data.ToString());
            var xml = new SceneXML();
            Schedule sch=null;
            if (url.Equals(""))
            {
                  sch = xml.parseSchedule(fileName);
            }
            else
            {
                uint size = new Http().download(url, fileName);
                if (size > 0)
                {
                    sch = xml.parseSchedule(fileName);
                }
            }

            String schName = "scene";
            DateTime now = DateTime.Now;
            int h = int.Parse(sch.startTime.Split(':')[0]);
            int m = int.Parse(sch.startTime.Split(':')[1]);
            DateTime startTime = new DateTime(now.Year, now.Month, now.Day, h,m,0);
            h = int.Parse(sch.endTime.Split(':')[0]);
            m = int.Parse(sch.endTime.Split(':')[1]);
            DateTime endTime = new DateTime(now.Year, now.Month, now.Day, h, m, 0);
            int inteval = sch.interval;
            if (this.schduleType == schType.device)
            {
                int min = startTime.Minute + inteval;
                int hour = startTime.Hour;
                if (min > 59)
                {
                    hour++;
                }
                endTime = new DateTime(now.Year, now.Month, now.Day, hour, min, 0);
                schName = "device";
            }

            start(schName, startTime, sch.weekDays);
            start("e_"+schName, endTime, sch.weekDays);
        }

        public void start(string name, DateTime time,ArrayList weekdays)
        {
            ScheduledEvent startSch = new ScheduledEvent(name + this.schdulerID, this.schGroup);
            startSch.Description = "schedule to:" + this.schdulerID;
            DateTime now = DateTime.Now;
            startSch.DateAndTime.SetAbsoluteEventTime(now.Year, now.Month, now.Day, time.Hour, time.Minute);
            calcRepeat(startSch, weekdays);
            startSch.Acknowledgeable = true;
            startSch.Persistent = false;
            startSch.AcknowledgeExpirationTimeout.Hour = 24*30*12;
            startSch.UserCallBack += new ScheduledEvent.UserEventCallBack(callback);
            startSch.Enable();
            CrestronConsole.PrintLine("Event create with {0} {1}:{2}", startSch.Name, startSch.DateAndTime.Hour, startSch.DateAndTime.Minute);
        }

        public void stop(int id)
        {
            IDictionaryEnumerator enumerator = (IDictionaryEnumerator)this.schGroup.ScheduledEvents.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.Equals("device" + id) || enumerator.Key.Equals("scene" + id) || enumerator.Key.Equals("e_device" + id) || enumerator.Key.Equals("e_scene" + id))
                {
                    ScheduledEvent eve = (ScheduledEvent)enumerator.Value;
                    eve.Disable();
                    this.schGroup.DeleteEvent(eve);
                }
            }
        }

        private void calcRepeat(ScheduledEvent se,ArrayList weekdays)
        {
            if (weekdays.Capacity == 7)
            {
                se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.All);
                return;
            }
            if (weekdays.Capacity == 2 && weekdays.Contains(0) && weekdays.Contains(6))
            {
                se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.Weekends);
                return;
            }

            if (weekdays.Capacity == 5 && weekdays.Contains(1) && weekdays.Contains(2) && weekdays.Contains(3) && weekdays.Contains(4) && weekdays.Contains(5))
            {
                se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.Workdays);
                return;
            }

            foreach (int day in weekdays)
            {
                switch (day)
                {
                    case 0:
                        se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.Sunday);
                        break;
                    case 1:
                        se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.Monday);
                        break;
                    case 2:
                        se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.Tuesday);
                        break;
                    case 3:
                        se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.Wednesday);
                        break;
                    case 4:
                        se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.Thursday);
                        break;
                    case 5:
                        se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.Friday);
                        break;
                    case 6:
                        se.Recurrence.Weekly(ScheduledEventCommon.eWeekDays.Saturday);
                        break;
                    default:
                        break;
                }
            }
        }

        public void callback(ScheduledEvent e, ScheduledEventCommon.eCallbackReason r)
        {
            CrestronConsole.PrintLine("scheduler at:{0}", DateTime.Now.ToString());
            string fileName = "\\NVRAM\\" + this.hostID + "_" + this.schdulerID + ".plist";
            var xml = new SceneXML();
            string deviceinfo = xml.parseSceneToDevices(fileName);
            if (e.Name == "scene" + this.schdulerID || e.Name == "device" + this.schdulerID)
            {
                if (openit != null)
                {
                    openit(deviceinfo);
                }
            }
            if (e.Name == "e_scene" + this.schdulerID || e.Name == "e_device" + this.schdulerID)
            {
                if (openit != null)
                {
                    closeit(deviceinfo);
                }
            }

        }
    }
}