﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.CompilerServices;

namespace FeintSDK
{
    public class Session
    {
        static SQLiteConnection connection;
        string key;
        Int64 id;
        public static Random random = new Random();
        static long maxId = -1;
        public Session()
        {
            if (connection == null)
            {
                if (!File.Exists("Session.sqlite"))
                {
                    SQLiteConnection.CreateFile("Session.sqlite");
                    connection = new SQLiteConnection("Data Source = Session.sqlite;");
                    connection.Open();
                    var command = new SQLiteCommand("CREATE TABLE \"Key\" (\"id\" INTEGER PRIMARY KEY  NOT NULL , \"key\" TEXT UNIQUE ,\"date\" DATETIME DEFAULT CURRENT_TIME)", connection);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand("CREATE TABLE Property(  id     INTEGER PRIMARY KEY  NOT NULL ,   name  TEXT,  value TEXT,   PropertyToKey INTEGER,   FOREIGN KEY(PropertyToKey) REFERENCES Key(id))", connection);
                    command.ExecuteNonQuery();
                }
                else
                {
                    connection = new SQLiteConnection("Data Source = Session.sqlite;");
                    connection.Open();
                }

            }

        }

        public string Start(string key)
        {
            string query = "SELECT * from 'key' where key='" + System.Security.SecurityElement.Escape(key) + "'";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(new SQLiteCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            var count = dataSet.Tables[0].Rows.Count;
            if ((count) == 1)
            {
                this.key = key;
                this.id = (Int64)dataSet.Tables[0].Rows[0][dataSet.Tables[0].Columns.IndexOf("id")];
                return key;
            }
            else
                return Start();
        }

        public string Start()
        {
            SHA1 sha1 = SHA1.Create();
            if (maxId == -1)
                maxId = getMaxIndex();
            string before = DateTime.Now.Ticks.ToString() + random.Next() + "" + maxId;
            maxId++;
            var hashed = SHA1Hash(before);
            try
            {
                var command = new SQLiteCommand("INSERT INTO 'main'.'Key' ('key') VALUES ('" + hashed + "')", connection);
                int res= command.ExecuteNonQuery();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(new SQLiteCommand("SELECT * from 'key' where key='" + System.Security.SecurityElement.Escape(hashed) + "'", connection));
                var dataSet = new DataSet();
                DataTable dataTable = new DataTable();
                adapter.Fill(dataSet);
                var count = dataSet.Tables[0].Rows.Count;
                if ((count) == 1)
                {
                    this.key = hashed;
                    this.id = int.Parse(dataSet.Tables[0].Rows[0]["id"].ToString());
                }
            }
            catch
            {
            }
            return hashed;
        }

        public string GetProperty(string name)
        {
            string query = "SELECT * FROM 'property' WHERE PropertyToKey='" + this.id + "' and name='" + System.Security.SecurityElement.Escape(name) + "'";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(new SQLiteCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);

            var count = dataSet.Tables[0].Rows.Count;
            if (count > 0)
                return dataSet.Tables[0].Rows[count - 1]["value"].ToString();
            else
                return null;
        }
        public void SetProperty(string name, string value)
        {
            string cmdString = "INSERT INTO 'main'.'Property' ('name','value','PropertyToKey') VALUES ('" + System.Security.SecurityElement.Escape(name) + "','" + System.Security.SecurityElement.Escape(value) + "','" + id + "')";
            var command = new SQLiteCommand(cmdString, connection);
            int res = command.ExecuteNonQuery();
        }
        public void UnsetProperty(String name)
        {
            string cmdString = "DELETE FROM 'property' where name='" + System.Security.SecurityElement.Escape(name) + "' and propertytokey='" + System.Security.SecurityElement.Escape(id.ToString()) + "'";
            var command = new SQLiteCommand(cmdString, connection);
            command.ExecuteNonQuery();
        }
        public String this[string key]
        {
            get
            {
                return this.GetProperty(key);
            }
            set
            {
                SetProperty(key, value);
            }
        }
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
        public static string SHA1Hash(string text)
        {
            SHA1 sha1 = SHA1.Create();
            sha1.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = sha1.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        public long getMaxIndex()
        {
            string query = "SELECT * from 'key' ORDER BY id DESC LIMIT 1";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(new SQLiteCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            var count = dataSet.Tables[0].Rows.Count;
            if (count > 0)
            {
                long id = (long)dataSet.Tables[0].Rows[0][dataSet.Tables[0].Columns.IndexOf("id")];
                return id;
            }
            else return 0;
        }
    }
}
