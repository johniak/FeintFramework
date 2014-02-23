using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public abstract class DatabaseHelper : ICloneable
    {
        private object lockDB = new object();
        private bool isLocked;
        public abstract void Connect(String name, String user, String password, String host, int port);
      //  DataTable Select(string table, List<WhereComponent> where);
        public abstract DataTable Select(string table, List<WhereComponent> where, List<DBJoinInformation> joins, long limitStart, long limitCount, string orderBy, bool ascending);
        public abstract long Count(string table, List<WhereComponent> where, List<DBJoinInformation> joins);
        public abstract Int64 Insert(string table, List<DBPair> what);
        public abstract void Update(string table, List<DBPair> what, Int64 id);
        public abstract void RemoveFromTable(string table, Int64 id);
        public abstract void CreateTable(string table, List<Column> collumns);
        public abstract void CreateTable(string table, List<Column> collumns, List<Foreign> foreigners);
        public abstract void CreateDatabase(String name);
        public abstract string getDBType(Type type);
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        protected void lockIt()
        {
            Monitor.Enter(lockDB);
        }
        protected void unlockIt()
        {
            Monitor.Exit(lockDB);
        }
        public bool IsLocked { get { return Monitor.IsEntered(lockDB); } }
        
    }
}
