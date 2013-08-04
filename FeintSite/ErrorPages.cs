using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeintSDK;
using DotLiquid;

namespace Site
{
    class ErrorPages
    {
        public static Response ExpectedPostMethod(Request request)
        {

            var response = new Response("error/error_post.html", Hash.FromAnonymousObject(new { message = "Hello World!" }));
            return response;
        }
    }
}
