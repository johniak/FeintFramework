using FeintORM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public class WhereBuilder<T>
    {
        List<WhereComponent> whereList = new List<WhereComponent>();
        List<DBJoinInformation> joins = new List<DBJoinInformation>();
        QueryBuilder<T> queryBuilder;
        public WhereBuilder(QueryBuilder<T> queryBuilder)
        {
            this.queryBuilder = queryBuilder;
        }
        /// <summary>
        /// Equals gets parameters and check if equals same
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereBuilder<T> Eq(string column, object value)
        {
            convertParameters(ref column, ref value);
            whereList.Add(new WhereComponent() { column = column, value = value.ToString(), operatorType = DBQueryOperators.Equals });
            return this;
        }

        /// <summary>
        /// Less then value
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereBuilder<T> Lt(string column, object value)
        {
            convertParameters(ref column, ref value);
            whereList.Add(new WhereComponent() { column = column, value = value.ToString(), operatorType = DBQueryOperators.Less });
            return this;
        }

        /// <summary>
        /// Greater then value 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereBuilder<T> Gt(string column, object value)
        {
            convertParameters(ref column, ref value);
            whereList.Add(new WhereComponent() { column = column, value = value.ToString(), operatorType = DBQueryOperators.Greater });
            return this;
        }

        /// <summary>
        /// Less or equals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereBuilder<T> Le(string column, object value)
        {
            convertParameters(ref column, ref value);
            whereList.Add(new WhereComponent() { column = column, value = value.ToString(), operatorType = DBQueryOperators.LessOrEquals });
            return this;
        }

        /// <summary>
        /// Greater or equals
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public WhereBuilder<T> Ge(string column, object value)
        {
            convertParameters(ref column, ref value);
            whereList.Add(new WhereComponent() { column = column, value = value.ToString(), operatorType = DBQueryOperators.GreaterOrEquals });
            return this;
        }

        /// <summary>
        /// And between to logic
        /// </summary>
        /// <returns></returns>
        public WhereBuilder<T> And()
        {
            whereList.Add(new WhereComponent() { operatorType = DBQueryOperators.And });
            return this;
        }


        /// <summary>
        /// Or between to logic
        /// </summary>
        /// <returns></returns>
        public WhereBuilder<T> Or()
        {
            whereList.Add(new WhereComponent() { operatorType = DBQueryOperators.Or });
            return this;
        }

        /// <summary>
        /// Convert paramters from object moddel to database relation create joins
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        private void convertParameters(ref string column, ref object value)
        {
            if (column.Contains("."))
            {
                var columns = column.Split('.');
                Type type = typeof(T);
                string lastAlias = "";
                for (int i = 0; i < columns.Length - 1; i++)
                {
                    String fk = "fk_" + type.GetProperty(columns[i], BindingFlags.Public | BindingFlags.Instance).Name;

                    int id = joins.Count;
                    joins.Add(new DBJoinInformation() { Table = FeintORM.GetInstance().Prefix + type.GetProperty(columns[i], BindingFlags.Public | BindingFlags.Instance).PropertyType.Name, Alias = columns[i] + id, LeftCollumn = (lastAlias.Length > 0 ? lastAlias + "." : "") + "fk_" + columns[i], RightCollumn = "Id" });
                    dynamic d = value;
                    lastAlias = columns[i] + id;
                    type = type.GetProperty(columns[i]).PropertyType;
                }
                if (FeintORM.GetInstance().Helper.getDBType(type.GetProperty(columns.Last(), BindingFlags.Public | BindingFlags.Instance).PropertyType) == null)
                {
                    int id = joins.Count;
                    joins.Add(new DBJoinInformation() { Table = FeintORM.GetInstance().Prefix + type.GetProperty(columns.Last(), BindingFlags.Public | BindingFlags.Instance).PropertyType.Name, Alias = columns.Last() + id, LeftCollumn = (lastAlias.Length > 0 ? lastAlias + "." : "") + "fk_" + columns.Last(), RightCollumn = "Id" });
                    dynamic d = value;
                    column = columns.Last() + id + ".Id";

                    value = d.Id;
                }
                else
                    column = joins[joins.Count - 1].Alias + "." + columns[columns.Length - 1];
            }
            else
            {
                if (FeintORM.GetInstance().Helper.getDBType(typeof(T).GetProperty(column, BindingFlags.Public | BindingFlags.Instance).PropertyType) == null)
                {
                    int id = joins.Count;
                    joins.Add(new DBJoinInformation() { Table = FeintORM.GetInstance().Prefix + typeof(T).GetProperty(column, BindingFlags.Public | BindingFlags.Instance).PropertyType.Name, Alias = column + id, LeftCollumn = "fk_" + column, RightCollumn = "Id" });
                    dynamic d = value;
                    column = column + id + ".Id";
                    value = d.Id;
                }
            }
        }

        /// <summary>
        /// Execute query and fill object
        /// </summary>
        /// <returns></returns>
        public List<T> Execute()
        {

            Stopwatch timer = new Stopwatch();
            DataTable table;
            if (joins.Count == 0)
                table = FeintORM.GetInstance().Helper.Select(FeintORM.GetInstance().Prefix + typeof(T).Name, whereList);
            else
                table = FeintORM.GetInstance().Helper.SelectWithJoin(FeintORM.GetInstance().Prefix + typeof(T).Name, whereList, joins);
            PropertyInfo[] pr = typeof(T).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            List<T> tets = new List<T>(table.Rows.Count); 
            timer.Start();
            foreach (DataRow row in table.Rows)
            {
                Object obj = Activator.CreateInstance(typeof(T));
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (table.Columns[i].ColumnName.StartsWith("fk_"))
                    {
                        PropertyInfo prop1 = getProperty(pr,table.Columns[i].ColumnName.Substring(3));
                          //  typeof(T).GetProperty(table.Columns[i].ColumnName.Substring(3), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (prop1 == null)
                            continue;
                        if (row.ItemArray[i].GetType() == typeof(Int64))
                        {
                            prop1.SetValue(obj, getForeign(prop1.PropertyType, (Int64)row.ItemArray[i]), null);
                        }
                        else if (row.ItemArray[i].GetType() == typeof(Int32))
                        {
                            prop1.SetValue(obj, getForeign(prop1.PropertyType, (Int32)row.ItemArray[i]), null);
                        }
                        else
                        {
                            prop1.SetValue(obj, getForeign(prop1.PropertyType, (Int64)(-1)), null);
                        }
                        continue;
                    }
                   
                    PropertyInfo prop = getProperty(pr, table.Columns[i].ColumnName);
                    //PropertyInfo prop =typeof(T).GetProperty(table.Columns[i].ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    
                    if (null != prop && prop.CanWrite)
                    {
                        if (prop.PropertyType != typeof(DateTime))
                        {
                            if (prop.PropertyType == typeof(Int32))
                                prop.SetValue(obj, Convert.ToInt32((Int64)row.ItemArray[i]), null);
                            else
                                prop.SetValue(obj, row.ItemArray[i], null);
                        }
                        else
                            prop.SetValue(obj, DateTime.Parse(row.ItemArray[i].ToString()), null);

                    }
                }
                tets.Add((T)obj);
            }
            timer.Stop();
            Log.E(timer.ElapsedMilliseconds);
            return tets;
        }
        PropertyInfo getProperty(PropertyInfo[] pi, String name)
        {
            foreach (var p in pi)
                if (p.Name.ToLower() == name)
                    return p;
            return null;
        }
        /// <summary>
        /// Fill foregins objects 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns>Object with type t and id equals id</returns>
        private dynamic getForeign(Type t, Int64 id)
        {
            if (id < 0)
                return null;
            List<WhereComponent> whereList = new List<WhereComponent>();
            whereList.Add(new WhereComponent() { column = "Id", value = id.ToString(), operatorType = DBQueryOperators.Equals });
            var table = FeintORM.GetInstance().Helper.Select(FeintORM.GetInstance().Prefix + t.Name, whereList);
            List<dynamic> tets = new List<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                Object obj = Activator.CreateInstance(t);
                for (int i = 0; i < row.ItemArray.Length; i++)
                {

                    if (table.Columns[i].ColumnName.StartsWith("fk_"))
                    {
                        PropertyInfo prop1 = obj.GetType().GetProperty(table.Columns[i].ColumnName.Substring(3), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (row.ItemArray[i].GetType() == typeof(Int64))
                        {
                            prop1.SetValue(obj, getForeign(prop1.PropertyType, (Int64)row.ItemArray[i]), null);
                        }
                        else
                        {
                            prop1.SetValue(obj, getForeign(prop1.PropertyType, (Int64)(-1)), null);
                        }
                        continue;
                    }
                    PropertyInfo prop = obj.GetType().GetProperty(table.Columns[i].ColumnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanWrite)
                    {
                        prop.SetValue(obj, row.ItemArray[i], null);
                    }
                }
                tets.Add(obj);
            }
            if (tets.Count > 0)
                return tets[0];
            else
                return null;
        }
    }
    public struct WhereComponent
    {
        public String column { get; set; }
        public String value { get; set; }
        public DBQueryOperators operatorType { get; set; }
    }
}
