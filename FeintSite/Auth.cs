using FeintSDK;
using Newtonsoft.Json;
using Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site
{
    class Auth : AOPAttribute
    {
        public override Response PreRequest(Request request)
        {
            if (User.IsLogged(request.Session))
                return null;
            else
                return new Response(request, "403") { Status = 403 };
        }

        public override void PostRequest(Request request)
        {
        }
    }
}
