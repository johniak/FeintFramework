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
        protected List<Assembly> assemblies;
        public DatabaseHelper Helper { get; protected set; }
        protected static FeintORM instance;
        protected DBSetting settings;
        List<String> tablesCreated;
        public String Prefix { get; set; }
        protected FeintORM(List<Assembly> assemblies, DBSetting settings)
        {
            this.assemblies = assemblies;
            this.Helper = settings.Helper;
            this.settings = settings;
            Helper.Connect(settings.Name, settings.User, settings.Password, settings.Host, settings.Port);
            Prefix = "feint_";

        }
        public static FeintORM GetInstance(List<Assembly> assemblies, DBSetting settings)
        {
            if (instance == null)
                instance = new FeintORM(assemblies, settings);
            return instance;
        }
        public static FeintORM GetInstance()
        {
            return instance;
        }
        IEnumerable<Type> getAllModelClass()
        {
            foreach (var assembly in assemblies)
            {
                foreach (var t in assembly.GetTypes())
                    if (t.BaseType == typeof(DBModel))
                        yield return t;
            }
        }
        public void CreateTablesFromModel()
        {
            Helper.CreateDatabase(settings.Name);
            var types = getAllModelClass();
            tablesCreated = new List<string>();
            foreach (var t in types)
            {
                if(tablesCreated.IndexOf(t.Name)<0)
                    createTable(t);
            }
        }
        private void createTable(Type t)
        {
            var tableName = Prefix+t.Name;
            List<Column> collumns = new List<Column>();
            List<Foreign> foreigners = new List<Foreign>();
            var properties = getPropertiesFromClass(t);
            // collumns.Add(new Collumn("id", "INTEGER", true));
            foreach (var p in properties)
            {
                var attr = (DBProperty)p.GetCustomAttribute(typeof(DBProperty));
                var collumn = new Column(p.Name, Helper.getDBType(p.PropertyType), attr.PrimaryKey, attr.AutoIncrement, attr.Unique, attr.AllowNull);
                collumns.Add(collumn);
            }
            var foreignersTypes = getForeignersFromClass(t);
            foreach (var f in foreignersTypes)
            {
                if (tablesCreated.IndexOf(f.PropertyType.GenericTypeArguments[0].Name) < 0)
                    createTable(f.PropertyType.GenericTypeArguments[0]);
                var collumn = new Column("fk_" + f.Name, "INTEGER");
                var foreignKey = new Foreign("fk_" + f.Name, Prefix + f.PropertyType.GenericTypeArguments[0].Name, "id") { Col = collumn };
                collumns.Add(collumn);
                foreigners.Add(foreignKey);
            }
            this.Helper.CreateTable(tableName, collumns, foreigners);

            tablesCreated.Add(t.Name);
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
                if (p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DBForeignKey<>))
                    yield return p;
        }

        


    }
}
