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
using System.Reflection.Emit;
namespace Feint.FeintORM
{
    public class WhereBuilder<T>
    {
        List<WhereComponent> whereList = new List<WhereComponent>();
        List<DBJoinInformation> joins = new List<DBJoinInformation>();
        List<JoinInfo> Foregins = new List<JoinInfo>();
        long limitStart=-1;
        long limitCount=-1;
        class JoinInfo
        {
            public DBJoinInformation DBJoin { get; set; }
            public List<string> pathToMember { get; set; }
            public PropertyInfo propertyInfo { get; set; }
        }
        QueryBuilder<T> queryBuilder;
        public WhereBuilder(QueryBuilder<T> queryBuilder)
        {
            getAllForeginesDependencies(typeof(T));
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

        public WhereBuilder<T> Limit(long count)
        {
            limitStart = -1;
            limitCount = count;
            return this;
        }
        public WhereBuilder<T> Limit(long start,long count)
        {
            limitStart = start;
            limitCount = count;
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
					joins.Add(new DBJoinInformation() { Table = FeintORM.GetInstance().Prefix + type.GetProperty(columns[i], BindingFlags.Public | BindingFlags.Instance).PropertyType.GetGenericArguments()[0].Name, Alias = columns[i] + id, LeftCollumn = (lastAlias.Length > 0 ? lastAlias + "." : "") + "fk_" + columns[i], RightCollumn = "Id" });
                    dynamic d = value;
                    lastAlias = columns[i] + id;
                    type = type.GetProperty(columns[i]).PropertyType.GetGenericArguments()[0];
                }
                if (FeintORM.GetInstance().Helper.getDBType(type.GetProperty(columns.Last(), BindingFlags.Public | BindingFlags.Instance).PropertyType) == null)
                {
                    int id = joins.Count;
					var col = type.GetProperty (columns.Last (), BindingFlags.Public | BindingFlags.Instance).PropertyType.GetGenericArguments () [0].Name;

					joins.Add(new DBJoinInformation() { Table = FeintORM.GetInstance().Prefix + type.GetProperty(columns.Last(), BindingFlags.Public | BindingFlags.Instance).PropertyType.GetGenericArguments()[0].Name	, Alias = columns.Last() + id, LeftCollumn = (lastAlias.Length > 0 ? lastAlias + "." : "") + "fk_" + columns.Last(), RightCollumn = "Id" });
                    dynamic d = ((dynamic)value).Value;
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
					joins.Add(new DBJoinInformation() { Table = FeintORM.GetInstance().Prefix + typeof(T).GetProperty(column, BindingFlags.Public | BindingFlags.Instance).PropertyType.GetGenericArguments()[0].Name, Alias = column + id, LeftCollumn = "fk_" + column, RightCollumn = "Id" });
                    dynamic d = value;
                    column = column + id + ".Id";
                    value = d.Id;
                }
            }
        }

        /// <summary>
        /// Execute query and fill object
        /// iterate by all properties of model class and assign values from rows
        /// implements lazy forgins keys
        /// </summary>
        /// <returns></returns>
        public List<T> Execute()
        {

            Stopwatch timer = new Stopwatch();
            DataTable table;
            if (joins.Count == 0)
                table = FeintORM.GetInstance().Helper.Select(FeintORM.GetInstance().Prefix + typeof(T).Name, whereList,null,limitStart,limitCount);
            else
                table = FeintORM.GetInstance().Helper.Select(FeintORM.GetInstance().Prefix + typeof(T).Name, whereList, joins,limitStart,limitCount);
            timer.Start();
            var pr = getPropertiesFromClass(typeof(T)); 
            var fr = getForeignersFromClass(typeof(T));

            List<T> tets = new List<T>(table.Rows.Count);

            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            Type int32Type = typeof(Int32);
            Type dateType = typeof(DateTime);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                columns.Add(table.Columns[i].ColumnName, i);
            }

            foreach (DataRow row in table.Rows)
            {
                Object obj = Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo p in pr)
                {
                    int i = columns[p.Name];
                    if (p.PropertyType != dateType)
                    {
						if (p.PropertyType == int32Type&&row.ItemArray[i].GetType()==typeof(Int64))
                        {
                            
                            p.SetValue(obj, Convert.ToInt32((Int64)row.ItemArray[i]), null);
                        }
                        else
                        {
                            p.SetValue(obj, row.ItemArray[i], null);
                        }
                    }
                    else
                    {
						DateTime dt = new DateTime (1970, 1, 1, 0, 0, 0, 0);
						dt= dt.AddSeconds (((long)row.ItemArray [i])/1000);
                        p.SetValue(obj,dt, null);
                    }
                }

                
                foreach (PropertyInfo f in fr)
                {
                    Object fobj = Activator.CreateInstance(f.PropertyType, true);
                    var value=row.ItemArray[columns["fk_" + f.Name]];
                    if (value!=null&&value.GetType() != typeof(DBNull))
						f.PropertyType.GetProperty("Id", BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public).SetValue(fobj, value);
                    f.SetValue(obj, fobj);
                }
                tets.Add((T)obj);
            }
            timer.Stop();
            Log.E(timer.ElapsedMilliseconds);
            return tets;
        }
        public long Count()
        {
            long count=0;
            if (joins.Count == 0)
                count = FeintORM.GetInstance().Helper.Count(FeintORM.GetInstance().Prefix + typeof(T).Name, whereList, null);
            else
                count = FeintORM.GetInstance().Helper.Count(FeintORM.GetInstance().Prefix + typeof(T).Name, whereList, joins);
            return count;
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
            var table = FeintORM.GetInstance().Helper.Select(FeintORM.GetInstance().Prefix + t.Name, whereList,null,limitStart,limitCount);
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
        private List<PropertyInfo> getPropertiesFromClass(Type t)
        {
            List<PropertyInfo> pi = new List<PropertyInfo>(10);
            foreach (var p in t.GetProperties())
                if (p.GetCustomAttributes(typeof(DBProperty), false).Length > 0)
                    pi.Add(p);
            return pi;
        }

        private List<PropertyInfo> getForeignersFromClass(Type t)
        {
            List<PropertyInfo> pi = new List<PropertyInfo>(10);
            foreach (var p in t.GetProperties())
                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DBForeignKey<>))
                    pi.Add(p);
            return pi;
        }
        void getAllForeginesDependencies(Type t)
        {
            List<String> path = new List<string>();
            var fr = getForeignersFromClass(t);
            foreach (var f in fr)
            {
                var p = new List<string>(path);
                p.Add(f.Name);
                Foregins.Add(new JoinInfo() { pathToMember = p, propertyInfo = f });
                getAllForeginesDependenciesN(f.PropertyType, p);
            }
        }
        void getAllForeginesDependenciesN(Type t, List<String> path)
        {
            var fr = getForeignersFromClass(t);
            foreach (var f in fr)
            {
                var p = new List<string>(path);
                p.Add(f.Name);
                Foregins.Add(new JoinInfo() { pathToMember = p, propertyInfo = f });
                getAllForeginesDependenciesN(f.PropertyType, p);
            }
        }
    }
    public struct WhereComponent
    {
        public String column { get; set; }
        public String value { get; set; }
        public DBQueryOperators operatorType { get; set; }
    }
}
