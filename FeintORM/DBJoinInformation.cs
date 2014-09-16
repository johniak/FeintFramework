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

        public override bool Equals(object obj)
        {
            if (!(obj is DBJoinInformation))
                return false;
            var obj1=(DBJoinInformation)obj;
            return Table == obj1.Table && LeftCollumn == obj1.LeftCollumn && RightCollumn == obj1.RightCollumn;
        }
    }
}
