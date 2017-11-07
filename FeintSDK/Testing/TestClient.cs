using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using FeintSDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;
using FeintSDK.Server;

namespace FeintSDK.Testing
{
    public class TestClient : BaseServer
    {
        public TestClient()
        {

        }

        public virtual Response HttpCall(string path, RequestMethod method, String body, Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }
            string contentType = null;
            headers.TryGetValue("Content-Type", out contentType);
            var request = new Request(path)
            {
                Body = body,
                ContentType = contentType,
                Method = method,
                MethodString = method.ToString()
            };
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            return this.HandleRequest(request);
        }
    }
}