using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UserModel : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class UserUsername : Attribute
    {
        public String Label { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class UserPassword :Attribute
    {
        public String Label { get; set; }
    }

}
