using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Reflection;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Feint.FeintORM
{
    public class DBModel
    {
        [DBProperty(true)]
        public Int64 Id { get; set; }

        /// <summary>
        /// Very slow, gets all rows and use selector on list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        [ObsoleteAttribute]
        public static List<T> get<T>(Func<T, bool> selector)
        {
            return getAll<T>().Where(selector).ToList<T>();
        }

        /// <summary>
        /// Very slow, gets all rows and use selector on list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        [ObsoleteAttribute]
        public static dynamic getOne<T>(Func<T, bool> selector)
        {
            var all = getAll<T>();
            var tets = all.Where(selector).ToList<T>();
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
            return Find<T>().Where().Execute();
        }
        public static List<T> Where<T>(Expression<Func<T, bool>> predicate)
        {
            WhereBuilder<T> wh=new QueryBuilder<T>().Where();
            //LogicalBinaryExpression name = predicate.Body.GetType().Name;
            dynamic operation = predicate.Body;
           // Find<T>().Where().Eq("", "");
            if (operation.Left.GetType().Name == "LogicalBinaryExpression")
            {
                
            }
            else
            {
                dynamic left = operation.Left;
                dynamic right = operation.Right;
                switch ((ExpressionType)operation.NodeType)
                {
                    case ExpressionType.Equal:
                        String name =((String)left.ToString()).Substring(left.ToString().IndexOf(".")+1);
                        object value= right.Value;
                        return wh.Eq(name, value).Execute();
                        break;
                }
            }
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
                model.Id = orm.Helper.Insert(orm.Prefix+model.GetType().Name, paramsDictionary);
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
            orm.Helper.Update(orm.Prefix + this.GetType().Name, paramsDictionary, this.Id);
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

