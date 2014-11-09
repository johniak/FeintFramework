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
        public Func<Request, Response> View { get; set; }
        public RequestMethod Method { get; set; }
        public Url(String urlMatch, Func<Request, Response> view)
        {
            this.UrlMatch = urlMatch;
            this.View = view;
            this.Method = RequestMethod.ALL;
        }
        public Url(String urlMatch, Func<Request, Response> view, RequestMethod method)
        {
            this.UrlMatch = urlMatch;
            this.View = view;
            this.Method = method;
        }
    }
    public enum RequestMethod
    {
        ALL, GET, POST, PUT, DELETE, Unknown
    }
}
