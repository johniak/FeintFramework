using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Reflection;
using System.Data;
using System.Diagnostics;

namespace Feint.FeintORM
{
    public class DBModel
    {
        [DBProperty(true)]
        public Int64 Id { get; set; }

        [ObsoleteAttribute]
        public static List<T> get<T>(Func<T, bool> selector)
        {
            return getAll<T>().Where(selector).ToList<T>();
        }

        [ObsoleteAttribute]
        public static dynamic getOne<T>(Func<T, bool> selector)
        {
            var tets = getAll<T>().Where(selector).ToList<T>();
            if (tets.Count > 0)
                return tets[0];
            return null;
        }
        public static T CreateFromPost<T>(Dictionary<String, String> post)
        {
            Type t = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (var p in post)
            {
                var property = t.GetProperty(p.Key);
                if (property == null)
                    continue;
                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(obj, int.Parse(p.Value));
                }
                else if (property.PropertyType == typeof(float))
                {
                    property.SetValue(obj, float.Parse(p.Value));
                }
                else if (property.PropertyType == typeof(double))
                {
                    property.SetValue(obj, double.Parse(p.Value));
                }
                else
                    property.SetValue(obj, p.Value);
            }
            return obj;
        }
        public static QueryBuilder<T> Find<T>()
        {
            return new QueryBuilder<T>();
        }
        public static List<T> getAll<T>()
        {
            List<T> h = new List<T>();
            SQLiteCommandBuilder trol = new SQLiteCommandBuilder();
            FeintORM orm = FeintORM.GetInstance();
            Dictionary<string, string> where = new Dictionary<string, string>();
            var table = orm.Helper.Select(typeof(T).Name, where);
            List<T> tets = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                Object obj = Activator.CreateInstance(typeof(T));
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (table.Columns[i].ColumnName.StartsWith("fk_"))
                    {
                        PropertyInfo prop1 = obj.GetType().GetProperty(table.Columns[i].ColumnName.Substring(3), BindingFlags.Public | BindingFlags.Instance);
                        if (row.ItemArray[i].GetType() == typeof(Int64))
                            prop1.SetValue(obj, getForeign(prop1.PropertyType, (Int64)row.ItemArray[i]), null);
                        else
                            prop1.SetValue(obj, getForeign(prop1.PropertyType, (Int64)(-1)), null);

                        continue;
                    }
                    PropertyInfo prop = obj.GetType().GetProperty(table.Columns[i].ColumnName, BindingFlags.Public | BindingFlags.Instance);
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
            return tets;
        }

        private static dynamic getForeign(Type t, Int64 id)
        {
            SQLiteCommandBuilder trol = new SQLiteCommandBuilder();
            FeintORM orm = FeintORM.GetInstance();
            Dictionary<string, string> where = new Dictionary<string, string>();
            where.Add("Id", id.ToString());
            var table = orm.Helper.Select(t.Name, where);
            List<dynamic> tets = new List<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                Object obj = Activator.CreateInstance(t);
                for (int i = 0; i < row.ItemArray.Length; i++)
                {

                    if (table.Columns[i].ColumnName.StartsWith("fk_"))
                    {
                        PropertyInfo prop1 = obj.GetType().GetProperty(table.Columns[i].ColumnName.Substring(3), BindingFlags.Public | BindingFlags.Instance);
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
                    PropertyInfo prop = obj.GetType().GetProperty(table.Columns[i].ColumnName, BindingFlags.Public | BindingFlags.Instance);
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
        public static void Add(DBModel model)
        {
            if (model.Id != 0)
                return;
            var prop = model.GetType().GetProperties();
            FeintORM orm = FeintORM.GetInstance();
            var paramsDictionary = new List<DBPair>();
            foreach (var p in prop)
            {
                if (p.GetCustomAttributes(typeof(DBForeignKey)).ToList<Attribute>().Count != 0)
                {
                    var fmodel = (DBModel)p.GetValue(model);
                    if (fmodel != null)
                    {
                        Add(fmodel);
                        paramsDictionary.Add(new DBPair() { Collumn = "fk_" + p.Name, Value = fmodel.Id.ToString() });
                    }
                }
                else
                {
                    if (p.GetCustomAttributes(typeof(DBProperty)).ToList<Attribute>().Count != 0 && p.Name != "Id")
                    {
                        if (p.PropertyType == typeof(DateTime))
                            paramsDictionary.Add(new DBPair() { Collumn = p.Name, Value = p.GetValue(model) == null ? "" : ((DateTime)p.GetValue(model)).ToString("yyyy-MM-dd hh:mm:ss") });
                        else if (p.PropertyType == typeof(bool)) paramsDictionary.Add(new DBPair() { Collumn = p.Name, Value = (((bool)p.GetValue(model))==true?1:0).ToString() });
                        else 
                            paramsDictionary.Add(new DBPair() { Collumn = p.Name, Value = p.GetValue(model) == null ? "" : p.GetValue(model).ToString() });
                    }

                }

            }
            try
            {
                model.Id = orm.Helper.Insert(model.GetType().Name, paramsDictionary);
            }
            catch (SQLiteException e)
            {
                if (e.ErrorCode != 19)
                    throw e;
                var mes = e.Message;
                int indexOfStart = mes.IndexOf("column") + "colSumn".Length;
                int len = mes.IndexOf(" ", indexOfStart) - indexOfStart;
                var col = mes.Substring(indexOfStart, len);
                throw new ORMNotUniqueException() { collumn = col };
            }

        }
        public void Save()
        {
            if (this.Id == 0)
            {
                Add(this);
                return;
            }
            var prop = this.GetType().GetProperties();
            FeintORM orm = FeintORM.GetInstance();
            var paramsDictionary = new List<DBPair>();
            foreach (var p in prop)
            {
                if (p.GetCustomAttributes(typeof(DBForeignKey)).ToList<Attribute>().Count != 0)
                {
                    var fmodel = (DBModel)p.GetValue(this);
                    fmodel.Save();
                    paramsDictionary.Add(new DBPair() { Collumn = "fk_" + p.Name, Value = fmodel.Id.ToString() });
                }
                if (p.GetCustomAttributes(typeof(DBProperty)).ToList<Attribute>().Count != 0 && p.Name != "Id")
                    paramsDictionary.Add(new DBPair() { Collumn = p.Name, Value = p.GetValue(this) == null ? "" : p.GetValue(this).ToString() });

            }
            orm.Helper.Update(this.GetType().Name, paramsDictionary, this.Id);
        }


        public void Remove()
        {
            if (Id == 0)
                return;
            FeintORM orm = FeintORM.GetInstance();
            orm.Helper.RemoveFromTable(this.GetType().Name, Id);
        }
    }

}

