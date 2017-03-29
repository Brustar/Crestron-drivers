using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Crestron.SimplSharp;
using Crestron.SimplSharp.SQLite;
using Crestron.SimplSharp.CrestronIO;

namespace EcloudUtils
{
    public class Db
    {
        public const string dbPath = "./ecloud.sqlite";
        //数据库连接
        SQLiteConnection m_dbConnection;

        public void init()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            m_dbConnection = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;");
            m_dbConnection.Open();
        }

        public void initDB(string sql)
        {
            init();
            createTable(sql);
        }

        private int save(string sql)
        {  
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            return command.ExecuteNonQuery();
        }

        //在指定数据库中创建一个table
        private int createTable(string sql)
        {
            return save(sql);
        }

         //插入一些数据
        public int insert(string sql)
        {
            return save(sql);
        }

        public int update(string sql)
        {
            return save(sql);
        }

        //使用sql查询语句，并显示结果
        public ArrayList query(string sql)
        {
            //string sql = "select * from highscores order by score desc";
            ArrayList list = new ArrayList();
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ArrayList ht = new ArrayList();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    ht.Add(reader[i].ToString());
                }
                list.Add(ht);
            }
            return list;
        }

        public string singleQuery(string sql)
        {
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader[0].ToString();
            }
            return "";
        }
    }
}