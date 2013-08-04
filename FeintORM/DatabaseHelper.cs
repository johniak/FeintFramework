using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Reflection;

namespace Feint.FeintORM
{
    public class DatabaseHelper
    {
        private static DatabaseHelper helper;
        SQLiteConnection connection;
        Dictionary<DBQueryOperators,String> queryOperators= new Dictionary<DBQueryOperators,String>();
        public DatabaseHelper(SQLiteConnection connection)
        {
            queryOperators.Add(DBQueryOperators.Equals, "=");
            queryOperators.Add(DBQueryOperators.Less, "<");
            queryOperators.Add(DBQueryOperators.Greater, ">");
            queryOperators.Add(DBQueryOperators.LessOrEquals, "<=");
            queryOperators.Add(DBQueryOperators.GreaterOrEquals, ">=");
            queryOperators.Add(DBQueryOperators.Or, "OR");
            queryOperators.Add(DBQueryOperators.And, "AND");
            this.connection = connection;
            connection.Open();
        }
        //public static DatabaseHelper getInstance(SQLiteConnection connection)
        //{
        //    if (helper == null)
        //        helper = new DatabaseHelper(connection);
        //    return helper;
        //}
        public static string Esc(string text)
        {
            return System.Security.SecurityElement.Escape (text);
        }
        public DataTable Select(string table,Dictionary<string,string> where)
        {
            string query="SELECT * from '" + System.Security.SecurityElement.Escape (table)+"'";
            if (where.Count != 0)
            {
                query += " WHERE ";
                foreach (var v in where)
                {
                    query += "" + Esc(v.Key) + "='" + Esc(v.Value) + "'";
                }
            }
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(new SQLiteCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            return dataSet.Tables[0];
        }
        public DataTable Select(string table, List<WhereComponent> where)
        {
            StringBuilder builder = new StringBuilder();
            string query = "SELECT * from '" + System.Security.SecurityElement.Escape(table) + "'";
            if (where.Count != 0)
            {
                query += " WHERE ";
                foreach (var w in where)
                {
                    if (w.operatorType == DBQueryOperators.And || w.operatorType == DBQueryOperators.Or)
                    {
                        query += " "+queryOperators[w.operatorType]+" ";
                        continue;
                    }
                    query += "" + Esc(w.column)+"" +queryOperators[w.operatorType]+ "'" + Esc(w.value) + "'";
                }
            }
            Console.WriteLine(query);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(new SQLiteCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            return dataSet.Tables[0];
        }
        public DataTable SelectWithJoin(string table, List<WhereComponent> where,List<DBJoinInformation> joins)
        {
            StringBuilder builder = new StringBuilder();
            string query = "SELECT * from '" + System.Security.SecurityElement.Escape(table) + "'";
            foreach(var join in joins)
            {
                query += " JOIN " + join.Table +" AS "+join.Alias+ " ON " + join.LeftCollumn + "=" + join.Alias+"."+join.RightCollumn + "";
            }
            if (where.Count != 0)
            {
                query += " WHERE ";
                foreach (var w in where)
                {
                    if (w.operatorType == DBQueryOperators.And || w.operatorType == DBQueryOperators.Or)
                    {
                        query += " " + queryOperators[w.operatorType] + " ";
                        continue;
                    }
                    query += "" + Esc(w.column) + "" + queryOperators[w.operatorType] + "'" + Esc(w.value) + "'";
                }
            }
            Console.WriteLine(query);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(new SQLiteCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            return dataSet.Tables[0];
        }
        public Int64 Insert(string table, List<DBPair> what)
        {
            var commandString= "INSERT INTO 'main'.'"+Esc(table)+"' ";
            string collumns="(";
            string values="VALUES(";
            foreach(var v in what)
            {
                collumns+="'"+Esc(v.Collumn)+"',";
                values+="'"+Esc(v.Value)+"',";
            }
            collumns=collumns.Substring(0,collumns.Length-1)+") ";
            values=values.Substring(0,values.Length-1)+")";
            commandString+=collumns+values;
            var command = new SQLiteCommand(commandString,connection);
            
            command.ExecuteNonQuery();
            string query="SELECT Max(ID) from '" + System.Security.SecurityElement.Escape (table)+"'";
           
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(new SQLiteCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            return  (Int64)dataSet.Tables[0].Rows[0][0];
        }
        public void Update(string table, List<DBPair> what,Int64 id)
        {
            var commandString = "UPDATE '" + Esc(table) + "' SET ";
            for(int i=0;i<what.Count;i++)
            {
                var v =what[i];
                commandString += Esc(v.Collumn) + "='" +Esc(v.Value) +"' ";
                if(i<what.Count-1)
                    commandString+=",";
            }
            commandString += "WHERE Id ='" + Esc(id.ToString())+"'"; 
            var command = new SQLiteCommand(commandString, connection);

            command.ExecuteNonQuery();
        }
        public void RemoveFromTable(string table, Int64 id)
        {
            string query = "DELETE FROM " + Esc(table) + " WHERE Id= '" + Esc(id.ToString()) + "'";
            var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
        }
        public void CreateTable(string table, List<Collumn> collumns)
        {
            //CREATE TABLE "Key" ("id" INTEGER PRIMARY KEY  NOT NULL  UNIQUE , "key" TEXT UNIQUE , "date" DATETIME DEFAULT CURRENT_TIME)

            var commandString = "CREATE TABLE IF NOT EXISTS '" + Esc(table) + "' (";
            for (int i = 0; i < collumns.Count; i++)
            {
                commandString += collumns[i].ToString();
                if (i + 1 < collumns.Count)
                    commandString += ",";
            }
            commandString += ")";
            var command = new SQLiteCommand(commandString,connection);
            command.ExecuteNonQuery();
        }
        public void CreateTable(string table, List<Collumn> collumns, List<Foreign> foreigners)
        {
            //CREATE TABLE "Key" ("id" INTEGER PRIMARY KEY  NOT NULL  UNIQUE , "key" TEXT UNIQUE , "date" DATETIME DEFAULT CURRENT_TIME)

            var commandString = "CREATE TABLE IF NOT EXISTS '" + Esc(table) + "' (";
            for (int i = 0; i < collumns.Count; i++)
            {
                commandString += collumns[i].ToString();
                if (i + 1 < collumns.Count || foreigners.Count>0)
                    commandString += ",";
            }
            for (int i = 0; i < foreigners.Count; i++)
            {
                commandString += foreigners[i].ToString();
                if (i + 1 < foreigners.Count)
                    commandString += ",";
            }
            commandString += ")";
            var command = new SQLiteCommand(commandString, connection);
            command.ExecuteNonQuery();
        }
    }
}
