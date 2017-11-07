using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FeintSDK
{
    public class Request
    {
        public String Url { get; set; }
        public RequestMethod Method { get; set; }

        public String MethodString
        {
            get
            {
                return Method.ToString();
            }
            set
            {
                var method = value.ToUpper().Trim();
                switch (method)
                {
                    case "GET":
                        Method = RequestMethod.GET;
                        break;
                    case "POST":
                        Method = RequestMethod.POST;
                        break;
                    case "PUT":
                        Method = RequestMethod.PUT;
                        break;
                    case "PATCH":
                        Method = RequestMethod.PATCH;
                        break;
                    case "DELETE":
                        Method = RequestMethod.DELETE;
                        break;
                }
            }
        }

        public String Body { get; set; }
        public String ContentType { get; set; }
        public Dictionary<String, String> FormData = new Dictionary<string, string>();
        public object Data;
        public Session Session { get; set; }
        public GroupCollection Variables { get; private set; }
        private readonly Dictionary<String, String> _headers = new Dictionary<string, string>();
        public Dictionary<String, String> Headers { get { return _headers; } }
        private readonly CookiesSet _cookiesSet = new CookiesSet();
        public CookiesSet Cookies { get { return _cookiesSet; } }
        public Request(String url)
        {
            this.Url = url;
        }
    }
}
