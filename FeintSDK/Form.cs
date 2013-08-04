using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    class Form
    {
        PropertyInfo[] properties;
        public string Method { get; set; }
        public string Action { get; set; }

        public Form(Type t)
        {
            properties = t.GetProperties();
        }

        public override string ToString()
        {
            return "";
        }
    }
}
