using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    class SessionProperty 
    {
        public int SessionPropertyId { get; set; }
        public SessionKey Owner { get; set; }
        public String Name { get; set; }
        public String Value { get; set; }

    }
}
