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
        Assembly assembly;
        String dbName;
        public DatabaseHelper Helper;
        static FeintORM instance;
        private FeintORM(Assembly assembly, String dbName)
        {
            this.assembly = assembly;
            this.dbName = dbName;
            if (!File.Exists(dbName))
                SQLiteConnection.CreateFile(dbName);
            this.Helper = new DatabaseHelper(new SQLiteConnection("Data Source = " + dbName + ";"));
            //CreateTablesFromModel();

        }
        public static FeintORM GetInstance(Assembly assembly, String dbName)
        {
            if (instance == null)
                instance = new FeintORM(assembly, dbName);
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
                    var collumn = new Collumn(p.Name, getDBType(p.PropertyType), attr.PrimaryKey, attr.AutoIncrement, attr.Unique, attr.AllowNull);
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

        public static string getDBType(Type type)
        {
            if (typeof(int) == type)
                return "INTEGER";
            if (typeof(bool) == type)
                return "BOOL";
            if (typeof(double) == type)
                return "DOUBLE";
            if (typeof(float) == type)
                return "FLOAT";
            if (typeof(char) == type)
                return "CHAR";
            if (typeof(string) == type)
                return "TEXT";
            if (typeof(Int64) == type)
                return "INTEGER";
            if (typeof(DateTime) == type)
                return "DATATIME";
            if (typeof(byte[]) == type)
                return "BLOB";
            return null;
        }


    }
}
