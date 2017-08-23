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
        public const string path = "/NVRAM/userdefault.txt";

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

            if (!File.Exists(Txt.path))
            {
                File.Create(Txt.path);
                return "";
            }
            else
            {
                FileStream fs = new FileStream(txtPath, FileMode.Create,FileAccess.Read,FileShare.ReadWrite);
                StreamReader objReader = new StreamReader(fs, System.Text.Encoding.Default);
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
                fs.Close();
                return sb.ToString();
            }
        }
    }
}