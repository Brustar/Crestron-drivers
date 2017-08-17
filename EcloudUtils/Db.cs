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
        private const string connectString = "Data Source=/NVRAM/ecloud.sqlite;Version=3;";

        public int initDB(string sql)
        {
            return createTable(sql);
        }

        private int save(string sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectString))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        int ret = command.ExecuteNonQuery();
                        return ret;
                    }
                    catch (SQLiteException e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
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
            using (SQLiteConnection conn = new SQLiteConnection(connectString))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        ArrayList list = new ArrayList();
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
                    catch (SQLiteException e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            
        }

        public string singleQuery(string sql)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectString))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        conn.Open();
                        string ret = "";
                        SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            ret = reader[0].ToString();
                        }
                        return ret;
                    }
                    catch (SQLiteException e)
                    {
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }

    }
}