using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeintSDK;
using Feint.FeintORM;
using DotLiquid;

namespace Site.Models
{
    class ToDoTask:DBModel
    {
        [DBForeignKey]
        public User Owner { get; set; }

        [DBProperty]
        public DateTime UpdatedDate { get; set; }
        
        [DBProperty]
        public DateTime CreatedDate { get; set; }
    }
}
