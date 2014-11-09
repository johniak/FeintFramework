using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    public class Cookie
    {
        public String Name { get; set; }
        public String Value { get; set; }
        public String Path { get; set; }
        public int ExperiationDate { get; set; }
        public Cookie(String name, String value, String path)
        {
            this.Name = name;
            this.Value = value;
            this.Path = path;
        }
        public Cookie()
        {
            Name = String.Empty;
            Value = String.Empty;
            Path = String.Empty;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Cookie)) 
                return false;
            var cookie =(Cookie)obj;
            return cookie.Name == this.Name && cookie.Path==this.Path;
        }
    }
}
