using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feint.FeintORM;

namespace FeintSDK
{
    class SessionProperty : DBModel
    {
        public DBForeignKey<SessionKey> Owner { get; set; }
        [DBProperty]
        public String Name { get; set; }
        [DBProperty]
        public String Value { get; set; }

    }
}
