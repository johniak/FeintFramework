using Feint.FeintORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    class SessionKey:DBModel
    {
        [DBProperty]
        public String Key { get; set; }

    }
}
