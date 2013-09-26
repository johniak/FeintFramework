using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Security.Cryptography;
using System.IO;

namespace FeintSDK
{
    public class Session
    {
        static SQLiteConnection connection;
        string key;
        Int64 id;

        public Session()
        {
            if (connection == null)
            {
                if (!File.Exists("Session.sqlite"))
                {
                    SQLiteConnection.CreateFile("Session.sqlite");
                    connection = new SQLiteConnection("Data Source = Session.sqlite;");
                    connection.Open();
                    var command = new SQLiteCommand("CREATE TABLE \"Key\" (\"id\" INTEGER PRIMARY KEY  NOT NULL  UNIQUE , \"key\" TEXT UNIQUE ,\"date\" DATETIME DEFAULT CURRENT_TIME)", connection);
                    command.ExecuteNonQuery();
                    command = new SQLiteCommand("CREATE TABLE Property(  id     INTEGER PRIMARY KEY  NOT NULL  UNIQUE,   name  TEXT,  value TEXT,   PropertyToKey INTEGER,   FOREIGN KEY(PropertyToKey) REFERENCES Key(id))", connection);
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
            string query ="SELECT * from 'key' where key='" + System.Security.SecurityElement.Escape(key) + "'";
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
            string before = DateTime.Now.Ticks.ToString() + new Random().Next();

            var hashed = Convert.ToBase64String(sha1.ComputeHash(ASCIIEncoding.ASCII.GetBytes(before)));
            var command = new SQLiteCommand("INSERT INTO 'main'.'Key' ('key') VALUES ('" + hashed + "')", connection);
            command.ExecuteNonQuery();
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
            return hashed;
        }
        public string GetProperty(string name)
        {
            string query ="SELECT * FROM 'property' WHERE PropertyToKey='" + this.id + "' and name='" + System.Security.SecurityElement.Escape(name) + "'";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(new SQLiteCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);

            var count = dataSet.Tables[0].Rows.Count;
            if (count != 1)
                return null;

            return dataSet.Tables[0].Rows[0]["value"].ToString();
        }
        public void SetProperty(string name, string value)
        {
            string cmdString = "INSERT INTO 'main'.'Property' ('name','value','PropertyToKey') VALUES ('" + System.Security.SecurityElement.Escape(name) + "','" + System.Security.SecurityElement.Escape(value) + "','" + id + "')";
            var command = new SQLiteCommand(cmdString, connection);
            command.ExecuteNonQuery();
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
    }
}
