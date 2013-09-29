using Feint.FeintORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Models
{
    class SampleModel: DBModel
    {
        [DBProperty]
        public int Priority { get; set; }

        [DBProperty]
        public String Message { get; set; }
    }
}
