using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public interface DatabaseHelper
    {
        void Connect(String name, String user, String password, String host, int port);
      //  DataTable Select(string table, List<WhereComponent> where);
        DataTable Select(string table, List<WhereComponent> where, List<DBJoinInformation> joins, long limitStart, long limitCount);
        long Count(string table, List<WhereComponent> where, List<DBJoinInformation> joins);
        Int64 Insert(string table, List<DBPair> what);
        void Update(string table, List<DBPair> what, Int64 id);
        void RemoveFromTable(string table, Int64 id);
        void CreateTable(string table, List<Column> collumns);
        void CreateTable(string table, List<Column> collumns, List<Foreign> foreigners);
        void CreateDatabase(String name);
        string getDBType(Type type);
    }
}
