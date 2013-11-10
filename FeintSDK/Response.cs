using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;
using System.IO;
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
            this.Data =  System.Text.Encoding.GetEncoding("iso-8859-2").GetBytes(data);
        }
        public Response(String name, Hash parameters)
        {
            Status = 200;
            FileStream fs = new FileStream("FeintSite/" + Settings.ViewsFolder + name, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fs, Encoding.UTF8);
            var contents = reader.ReadToEnd();
            fs.Close();
            Template template = Template.Parse(contents);
            String renderedTemplate = template.Render(parameters);
            this.Data = System.Text.Encoding.GetEncoding("iso-8859-2").GetBytes(renderedTemplate);
        }
        public static Response Redirect(String url)
        {
            Response r = new Response();
            r.redirectUrl = url;
            return r;
        }
    }
}
