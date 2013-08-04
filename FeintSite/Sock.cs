using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy;
using System.Net;
using FeintSDK;

namespace Site
{
    class Sock
    {
        public Sock()
        {
            var server = new WebSocketServer(81, IPAddress.Any);
            server.OnConnected = onConnected;
            server.OnReceive = onRecive;
            server.Start();
        }

        void onConnected(Alchemy.Classes.UserContext context)
        {

        }
        void onRecive(Alchemy.Classes.UserContext context)
        {
            var c = context.Header.Cookies["session"];
            if (c == null)
                return;
            Session s=new Session();
            string key = s.Start(c.Value);
            if (c.Value != key)
                return;
        }
    }
}
