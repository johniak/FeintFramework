using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Reflection;
using System.IO;
namespace Feint.FeintORM
{
    public class FeintORM
    {
        protected Assembly assembly;
        public DatabaseHelper Helper { get; protected set; }
        protected static FeintORM instance;
        protected DBSetting settings;
        protected FeintORM(Assembly assembly, DBSetting settings)
        {
            this.assembly = assembly;
            this.Helper = settings.Helper;
            Helper.Connect(settings.Name, settings.User, settings.Password, settings.Host, settings.Port);
            
            //CreateTablesFromModel();

        }
        public static FeintORM GetInstance(Assembly assembly, DBSetting settings)
        {
            if (instance == null)
                instance = new FeintORM(assembly, settings);
            return instance;
        }
        public static FeintORM GetInstance()
        {
            return instance;
        }
        IEnumerable<Type> getAllModelClass()
        {
            foreach (var t in assembly.GetTypes())
                if (t.BaseType == typeof(DBModel))
                    yield return t;
        }
        public void CreateTablesFromModel()
        {
            Helper.CreateDatabase(settings.Name);
            var types = getAllModelClass();
            foreach (var t in types)
            {
                var tableName = t.Name;
                List<Collumn> collumns = new List<Collumn>();
                List<Foreign> foreigners = new List<Foreign>();
                var properties = getPropertiesFromClass(t);
                // collumns.Add(new Collumn("id", "INTEGER", true));
                foreach (var p in properties)
                {
                    var attr = (DBProperty)p.GetCustomAttribute(typeof(DBProperty));
                    var collumn = new Collumn(p.Name, Helper.getDBType(p.PropertyType), attr.PrimaryKey, attr.AutoIncrement, attr.Unique, attr.AllowNull);
                    collumns.Add(collumn);
                }
                var foreignersTypes = getForeignersFromClass(t);
                foreach (var f in foreignersTypes)
                {
                    var collumn = new Collumn("fk_" + f.Name, "INTEGER");
                    var foreignKey = new Foreign("fk_" + f.Name, f.PropertyType.Name, "id");
                    collumns.Add(collumn);
                    foreigners.Add(foreignKey);
                }
                this.Helper.CreateTable(tableName, collumns, foreigners);
            }
        }
        private IEnumerable<PropertyInfo> getPropertiesFromClass(Type t)
        {
            foreach (var p in t.GetProperties())
                if (p.GetCustomAttributes(typeof(DBProperty), false).Length > 0)
                    yield return p;
        }
        private IEnumerable<PropertyInfo> getForeignersFromClass(Type t)
        {
            foreach (var p in t.GetProperties())
                if (p.GetCustomAttributes(typeof(DBForeignKey), false).Length > 0)
                    yield return p;
        }

        


    }
}
