using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feint.FeintORM
{
    public class Foreign
    {
       public string Name { get; set; }
       public string Table { get; set; }
       public string Collumn { get; set; }
       public Collumn Col { get; set; }
       public Foreign(string name,string table ,string collumn)
       {
           this.Name = name;
           this.Table = table;
           this.Collumn = collumn;
       }
       public override string ToString()
       {
           return "FOREIGN KEY("+Name+") REFERENCES "+Table+"("+Collumn+")";
       }
    }
}
