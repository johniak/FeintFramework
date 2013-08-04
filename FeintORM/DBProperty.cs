using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DBProperty : System.Attribute
    {
        public bool PrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
        public bool Unique { get; set; }
        public bool AllowNull { get; set; }
        public bool TypedByProperties { get; set; }
        public DBProperty()
        {
            this.AutoIncrement = false;
            this.Unique = false;
            this.AllowNull = true;
        }
        public DBProperty(bool primaryKey)
        {
            this.PrimaryKey = primaryKey;
            this.AutoIncrement = true;
            this.Unique = true;
            this.AllowNull = false;
        }
        public DBProperty(bool primaryKey, bool autoIncrement, bool unique, bool allowNull)
        {
            this.PrimaryKey = primaryKey;
            this.AutoIncrement = autoIncrement;
            this.Unique = unique;
            this.AllowNull = allowNull;
        }
    }
}
