using FeintSDK;
using Newtonsoft.Json;
using Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    class ApiAuth : AOPAttribute
    {
        public override Response PreRequest(Request req)
        {
            if (User.IsLogged(req.Session))
                return null;
            else
                return new Response(JsonConvert.SerializeObject(Errors.NotLoggedIn)) {Status=403};
        }

        public override void PostRequest(Request req)
        {
        }
    }
}
