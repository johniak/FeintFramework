using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public class Collumn
    {
        public String Name { get; protected set; }
        public String Type { get; protected set; }
        public bool PrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
        public bool Unique { get; set; }
        public bool AllowNull { get; set; }

        public Collumn(String name, String type)
        {
            this.Name = SQLiteDatabaseHelper.Esc(name);
            this.Type = SQLiteDatabaseHelper.Esc(type);
            this.AutoIncrement = false;
            this.Unique = false;
            this.AllowNull = true;
        }

        public Collumn(String name, String type, bool primaryKey)
        {
            this.Name = SQLiteDatabaseHelper.Esc(name);
            this.Type = SQLiteDatabaseHelper.Esc(type);
            this.PrimaryKey = primaryKey;
            this.AutoIncrement = true;
            this.Unique = true;
            this.AllowNull = false;
        }
        public Collumn(String name, String type, bool primaryKey, bool autoIncrement, bool unique, bool allowNull)
        {
            this.Name = SQLiteDatabaseHelper.Esc(name);
            this.Type = SQLiteDatabaseHelper.Esc(type);
            this.PrimaryKey = primaryKey;
            this.AutoIncrement = autoIncrement;
            this.Unique = unique;
            this.AllowNull = allowNull;
        }
        public override string ToString()
        {
            string cmd = "" + this.Name + " " + this.Type + " ";
            if (PrimaryKey)
                cmd += "PRIMARY KEY ";
            if (AutoIncrement)
                cmd += "AUTOINCREMENT ";
            if (Unique)
                cmd += "UNIQUE ";
            if (!AllowNull)
                cmd += "NOT NULL";
            return cmd;
        }

    }
}
