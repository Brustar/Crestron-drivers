using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronXml;
using Crestron.SimplSharp.CrestronXmlLinq;

namespace EcloudUtils
{
    public class SceneXML
    {
        private XElement readXML(string filePath)
		{
            XmlReader reader = XmlReader.Create(filePath);
            XElement xe = XElement.Load(reader);
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
                ret = ret + json.Substring(0, ret.Length - 1) +"|";
            }
            return ret.Substring(0,ret.Length-1);
        }

        public string parseKeyToSceneID(string filePath,int key)
        {
            CrestronConsole.PrintLine("filePath:" + filePath);
            XElement xe = readXML(filePath);
            CrestronConsole.PrintLine("-------");
            IEnumerable<XElement> elements = from e in xe.Descendants("dict")
                                             where (e.Element("key").Value == "keyID"
                                             && e.Elements("integer").First().Value == Convert.ToString(key))
                                             select e;
            CrestronConsole.PrintLine("-------1" + elements.First().Elements("integer").Last().Value);
            return elements.First().Elements("integer").Last().Value;
        }

    }
}
