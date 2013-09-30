using FeintORM;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public class PostgreSQLDatabaseHelper : DatabaseHelper
    {
        Dictionary<DBQueryOperators, String> queryOperators = new Dictionary<DBQueryOperators, String>();
        NpgsqlConnection connection;
        String connectionString;

        public PostgreSQLDatabaseHelper()
        {
            queryOperators.Add(DBQueryOperators.Equals, "=");
            queryOperators.Add(DBQueryOperators.NotEquals, "!=");
            queryOperators.Add(DBQueryOperators.Less, "<");
            queryOperators.Add(DBQueryOperators.Greater, ">");
            queryOperators.Add(DBQueryOperators.LessOrEquals, "<=");
            queryOperators.Add(DBQueryOperators.GreaterOrEquals, ">=");
            queryOperators.Add(DBQueryOperators.Or, "OR");
            queryOperators.Add(DBQueryOperators.And, "AND");
            queryOperators.Add(DBQueryOperators.Like, "LIKE");
        }

        public void Connect(String name, String user, String password, String host, int port)
        {
            connectionString = "Server=" + host + ";Port=" + port + ";User Id=" + user + ";Password=" + password + ";Database=" + name + ";";
            connection = new NpgsqlConnection("Server=" + host + ";Port=" + port + ";User Id=" + user + ";Password=" + password + ";");
            connection.Open();
        }

        public static string Esc(string text)
        {
            return System.Security.SecurityElement.Escape(text);
        }

        public DataTable Select(string table, List<WhereComponent> where)
        {
            StringBuilder builder = new StringBuilder();
            string query = "SELECT * from " + System.Security.SecurityElement.Escape(table) + "";
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
            Log.D(query);
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connection);
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            return dataSet.Tables[0];
        }

        public DataTable Select(string table, List<WhereComponent> where, List<DBJoinInformation> joins, long limitStart, long limitCount,string orderBy,bool ascending)
        {
            StringBuilder builder = new StringBuilder();
            string query = "SELECT * FROM " + System.Security.SecurityElement.Escape(table) + "";
            if (joins != null)
            {
                foreach (var join in joins)
                {
                    query += " JOIN " + join.Table + " AS " + join.Alias + " ON " + join.LeftCollumn + "=" + join.Alias + "." + join.RightCollumn + "";
                }
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
                    query += "" + Esc(w.column) + " " + queryOperators[w.operatorType] + " '" + Esc(w.value) + "'";
                }
            }
            if (orderBy != null)
            {
                query += "  ORDER BY " + orderBy + (ascending?" ASC":" DESC");
            }
            if (limitStart != -1 && limitCount != -1)
            {
                query += " LIMIT " + limitCount + " OFFSET " + limitStart;
            }
            else if (limitCount != -1)
            {
                query += " LIMIT " + limitCount;
            }
            Log.D(query);
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(new NpgsqlCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            return dataSet.Tables[0];
        }
        
       public  long Count(string table, List<WhereComponent> where, List<DBJoinInformation> joins)
        {
            StringBuilder builder = new StringBuilder();
            string query = "SELECT COUNT (*) from " + System.Security.SecurityElement.Escape(table) + "";
            if (joins != null)
            {
                foreach (var join in joins)
                {
                    query += " JOIN " + join.Table + " AS " + join.Alias + " ON " + join.LeftCollumn + "=" + join.Alias + "." + join.RightCollumn + "";
                }
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
            
            Log.D(query);
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(new NpgsqlCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);
            return (long) dataSet.Tables[0].Rows[0].ItemArray[0];
        }

        public Int64 Insert(string table, List<DBPair> what)
        {
            var commandString = "INSERT INTO " + Esc(table) + " ";
            string collumns = "(";
            string values = "VALUES(";
            foreach (var v in what)
            {
                collumns += "" + Esc(v.Collumn) + ",";
                values += "'" + Esc(v.Value) + "',";
            }
            collumns = collumns.Substring(0, collumns.Length - 1) + ") ";
            values = values.Substring(0, values.Length - 1) + ")";
            commandString += collumns + values;
            var command = new NpgsqlCommand(commandString, connection);
            command.ExecuteNonQuery();
            string query = "SELECT Max(ID) from " + System.Security.SecurityElement.Escape(table) + "";
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(new NpgsqlCommand(query, connection));
            var dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataSet);

            return (int)dataSet.Tables[0].Rows[0][0];
        }

        public void Update(string table, List<DBPair> what, Int64 id)
        {
            var commandString = "UPDATE " + Esc(table) + " SET ";
            for (int i = 0; i < what.Count; i++)
            {
                var v = what[i];
                commandString += Esc(v.Collumn) + "='" + Esc(v.Value) + "' ";
                if (i < what.Count - 1)
                    commandString += ",";
            }
            commandString += "WHERE Id ='" + Esc(id.ToString()) + "'";
            var command = new NpgsqlCommand(commandString, connection);
            command.ExecuteNonQuery();
        }

        public void RemoveFromTable(string table, Int64 id)
        {
            string query = "DELETE FROM " + Esc(table) + " WHERE Id= '" + Esc(id.ToString()) + "'";
            var command = new NpgsqlCommand(query, connection);
            Log.D(query);
            command.ExecuteNonQuery();
        }

        public void CreateTable(string table, List<Column> collumns)
        {
            var commandString = "CREATE TABLE IF NOT EXISTS '" + Esc(table) + "' (";
            for (int i = 0; i < collumns.Count; i++)
            {
                commandString += collumns[i].ToString();
                if (i + 1 < collumns.Count)
                    commandString += ",";
            }
            commandString += ")";
            var command = new NpgsqlCommand(commandString, connection);
            command.ExecuteNonQuery();
        }

        public void CreateTable(string table, List<Column> collumns, List<Foreign> foreigners)
        {
            var commandString = "CREATE TABLE IF NOT EXISTS " + Esc(table) + " (";
            for (int i = 0; i < collumns.Count; i++)
            {
                commandString += columnToString(collumns[i]);
                Foreign f;
                if ((f = getForeginForCollumn(collumns[i], foreigners)) != null)
                {
                    commandString += "references " + Esc(f.Table) + "(" + Esc(f.Collumn) + ")";
                }
                if (i + 1 < collumns.Count)
                    commandString += ",";
            }
            commandString += ")";
            var command = new NpgsqlCommand(commandString, connection);
            command.ExecuteNonQuery();
        }
        private Foreign getForeginForCollumn(Column c, List<Foreign> foreigners)
        {
            foreach (var f in foreigners)
            {
                if (f.Col == c)
                    return f;
            }
            return null;
        }
        public string columnToString(Column col)
        {
            string cmd = "" + col.Name + " ";
            if (col.AutoIncrement)
            {
                cmd += "serial" + " ";
            }
            else
            {
                cmd += col.Type + " ";
            }
            if (col.PrimaryKey)
                cmd += "PRIMARY KEY ";
            if (col.Unique)
                cmd += "UNIQUE ";
            if (!col.AllowNull)
                cmd += "NOT NULL";
            return cmd;
        }
        public void CreateDatabase(String name)
        {
            string commandString = "CREATE DATABASE " + Esc(name);
            var command = new NpgsqlCommand(commandString, connection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
            }
            connection.Close();
            connection = new NpgsqlConnection(connectionString);
            connection.Open();
        }

        public string getDBType(Type type)
        {
            if (typeof(int) == type)
                return "INTEGER";
            if (typeof(bool) == type)
                return "BOOL";
            if (typeof(double) == type)
                return "DOUBLE PRECISION";
            if (typeof(float) == type)
                return "REAL";
            if (typeof(char) == type)
                return "CHAR";
            if (typeof(string) == type)
                return "TEXT";
            if (typeof(Int64) == type)
                return "INTEGER";
            if (typeof(DateTime) == type)
                return "BIGINT";
            if (typeof(byte[]) == type)
                return "BYTES";
            return null;
        }
    }
}
