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
        private Response()
        {

        }
        public Response(byte[] data)
        {
            this.Data = data;
        }
        public Response(string data)
        {
            this.Data =  System.Text.Encoding.GetEncoding("iso-8859-2").GetBytes(data);
        }
        public Response(String name, Hash parameters)
        {
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
