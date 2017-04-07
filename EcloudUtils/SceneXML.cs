using System;
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
        private static string readXML(string filePath)
		{
            XmlReader reader = XmlReader.Create(filePath);
            XElement xe = XElement.Load(reader);
			IEnumerable<XElement> elements = from ele in 
				xe.Elements("dict").Elements("array").Elements("dict")
				                                                select ele;
			string ret = "[";
			foreach(var e in elements)
			{
				string json = "{";
				foreach (var l in e.Elements())
				{
					if (l.Name == "key")
					{
						json = json + "\"" + l.Value + "\":";
					}
					else if (l.Name == "true" || l.Name == "false")
					{

						json = json + "\"" + l.Name + "\",";
					}
					else
					{
						json = json + "\"" + l.Value + "\",";
					}

				}
				json = json.Substring(0,json.Length-1) + "}";
				ret = ret + json + ",";
			}
			return ret.Substring(0, ret.Length - 1) + "]";
		}
    }
}
