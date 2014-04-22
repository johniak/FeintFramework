using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public struct DBSetting
    {
       public DatabaseHelper Helper { get; set; }
       public String Name { get; set; }
       public String User { get; set; }
       public String Password { get; set; }
       public String Host { get; set; }
       public int Port { get; set; }
       public int ConnectionsCount { get; set; }
    }
}
