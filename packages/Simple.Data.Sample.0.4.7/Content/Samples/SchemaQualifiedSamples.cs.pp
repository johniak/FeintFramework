using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Data;

namespace $rootnamespace$.Samples
{
    class SchemaQualifiedSamples
    {
        public static void QualifiedTableExample()
        {
            var db = Database.Open(); // Opens the default database, as specified in config
            foreach (var customer in db.dbo.Customers.All())
            {
                Console.WriteLine(customer.Name);
            }
        }

        public static void QualifiedProcedureCallExample()
        {
            // This doesn't work yet. Fixing in 0.4.8.
            //var db = Database.Open();
            //foreach (var customer in db.dbo.GetCustomers())
            //{
            //    Console.WriteLine(customer.Name);
            //}
        }
    }
}
