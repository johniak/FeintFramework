using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    public class Url
    {
        public String UrlMatch { get; set; }
        public Func<Request,Response> View { get; set; }
        public Url(String urlMatch, Func<Request, Response> view)
        {
            this.UrlMatch = urlMatch;
            this.View = view;
        }
    }
}
