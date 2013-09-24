using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public class QueryBuilderDynamic
    {
        Type t;
        public QueryBuilderDynamic(Type t)
        {
            this.t = t;
        }

        public WhereBuilderDynamic Where()
        {
            return new WhereBuilderDynamic(this, t);
        }
    }
}
