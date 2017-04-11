using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;

namespace EcloudUtils
{
    public class Txt
    {
        public const string path = "./userdefault.txt";

        public static void write(string txtPath, string txtStr)
        {
            FileStream fs = new FileStream(txtPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(txtStr);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        public static string read(string txtPath)
        {
            StringBuilder sb = new StringBuilder();
            StreamReader objReader;
            if (!File.Exists(Txt.path))
            {
                FileStream stream = File.Create(Txt.path);
                objReader = new StreamReader(stream, System.Text.Encoding.Default);
            }
            else
            {
                objReader = new StreamReader(txtPath, System.Text.Encoding.Default);
            }

            string sLine = "";
            while (!objReader.EndOfStream)
            {
                sLine = objReader.ReadLine();
                if (sLine != null && !sLine.Equals(""))
                {
                    sb.Append(sLine + "\r\n");
                }
            }
            objReader.Close();
            return sb.ToString();
        }
    }
}