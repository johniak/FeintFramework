using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public struct DBJoinInformation
    {
        public String Table { get; set; }
        public String Alias { get; set; }
        public String LeftCollumn { get; set; }
        public String RightCollumn { get; set; }
    }
}
