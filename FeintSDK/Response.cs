using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FeintTemplateEngine;

namespace FeintSDK
{
    public class Response
    {
        public byte[] Data { get; set; }
        private string redirectUrl;
        public int Status { get; set; }
        public string MimeType { get; set; }
        private Response()
        {
            Status = 200;
        }
        public Response(byte[] data)
        {
            Status = 200;
            this.Data = data;
        }
        public Response(string data)
        {
            Status = 200;
            this.Data = Encoding.UTF8.GetBytes(data);
        }
        public Response(String name, Object parameters)
        {
            Status = 200;
            TemplateEngine templateEngine = TemplateEngine.FromFile(name, parameters);
            this.Data = Encoding.UTF8.GetBytes(templateEngine.Render());
        }
        public static Response Redirect(String url)
        {
            Response r = new Response();
            r.redirectUrl = url;
            return r;
        }
    }
}
