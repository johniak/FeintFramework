using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FeintSDK
{
    public class Request
    {
        public String Url { get; set; }
        public String Method { get; set; }
        public Dictionary<String, String> POST=new Dictionary<string,string>();
        public Session Session { get; set; }
        public GroupCollection variables { get; private set; }
        public Request(String url)
        {
            this.Url = url;
        }
    }
}
