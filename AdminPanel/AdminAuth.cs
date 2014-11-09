using FeintSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel
{
    class AdminAuth : AOPAttribute
    {
        public override Response PreRequest(Request req)
        {
            string logged=req.Session.GetProperty(Site.SessionLoggedKey);
            if (logged != null && int.Parse(logged) != -1)
            {
                return null;
            }
            else
            {
                return Response.Redirect(req, Site.AdminLoginUrl);
            }
            //throw new NotImplementedException();
            
        }

        public override void PostRequest(Request req)
        {
            //throw new NotImplementedException();
        }

    }
}
