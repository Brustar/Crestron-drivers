using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronXml;
using Crestron.SimplSharp.CrestronXmlLinq;

namespace EcloudUtils
{
    public class SceneXML
    {
        private string deleteDTD(string filePath)
        {
            string text = "";
            //用一个读出流去读里面的数据

            using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
            {
                //读一行
                string line = reader.ReadLine();
                while (line != null)
                {
                    //如果这一行里面有abe这三个字符，就不加入到text中，如果没有就加入
                    if (line.IndexOf("<!DOCTYPE") < 0)
                    {
                        text += line;
                    }
                    //一行一行读
                    line = reader.ReadLine();
                }
            }

            return text;
        }

        private XElement readXML(string filePath)
		{
            string xml = deleteDTD(filePath);
            //XmlReader reader = XmlReader.Create(filePath);
            XElement xe = XElement.Parse(xml);//.Load(reader);
            return xe;
		}

        public string parseSceneToDevices(string filePath)
        {
            XElement xe = readXML(filePath);
            IEnumerable<XElement> elements = from ele in
                                                 xe.Elements("dict").Elements("array").Elements("dict")
                                             select ele;
            string ret = "";
            foreach (var e in elements)
            {
                string json = "";
                foreach (var l in e.Elements())
                {
                    if (l.Name == "key")
                    {
                        json = json + l.Value + ":";
                    }
                    else if (l.Name == "true" || l.Name == "false")
                    {

                        json = json + l.Name + ",";
                    }
                    else
                    {
                        json = json + l.Value + ",";
                    }

                }
                ret = ret + json.Substring(0, json.Length - 1) + "|";
            }
            return ret.Substring(0,ret.Length-1);
        }

        public string parseKeyToSceneID(string filePath,int key)
        {
            XElement xe = readXML(filePath);
            IEnumerable<XElement> elements = from e in xe.Descendants("dict")
                                             where (e.Element("key").Value == "keyID"
                                             && e.Elements("integer").First().Value == Convert.ToString(key))
                                             select e;
            return elements.First().Elements("integer").Last().Value;
        }

        public Schedule parseSchedule(string filePath)
        {
			XmlReaderSettings settings = new XmlReaderSettings();
			XmlReader reader = XmlReader.Create(filePath, settings);
			XElement xe = XElement.Load(reader);

			IEnumerable<XElement> elements = from ele in
				xe.Elements("dict").Elements("array").Last().Elements("dict")
											 select ele;
			string start = elements.First().Elements("string").Last().Value;

			string end = elements.First().Elements("string").First().Value;
			string interval = elements.First().Elements("integer").First().Value;
			IEnumerable<XElement> weekdays = elements.First().Elements("array").First().Elements("integer");

			ArrayList list = new ArrayList();
			foreach (var e in weekdays)
			{
				list.Add(e.Value);
			}

            Schedule sch = new Schedule();
			sch.startTime=start;
            sch.endTime=end;
            sch.interval = int.Parse(interval);
            sch.weekDays=list;
            return sch;
        }

    }
}
