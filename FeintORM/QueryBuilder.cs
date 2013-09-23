using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
   public class QueryBuilder<T>
    {

        String orderBy;
        bool desc;

        public WhereBuilder<T> Where()
        {
            return new WhereBuilder<T>(this);
        }


    }


    
}
