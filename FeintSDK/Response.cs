using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace FeintSDK
{
    public class Response
    {
        public byte[] Data { get; set; }
        public int Status { get; set; }
        public string ContentType { get; set; }
        private readonly Dictionary<String, String> _headers = new Dictionary<string, string>();
        public Dictionary<String, String> Headers { get { return _headers; } }
        private readonly CookiesSet _cookiesSet = new CookiesSet();
        public CookiesSet Cookies { get { return _cookiesSet; } }
        public bool IsRedirect { get { return RedirectUrl != null; } }
        public string RedirectUrl { get; private set; }

        protected Response()
        {
            Status = 200;
        }
        public Response(byte[] data)
        {
            Status = 200;
            this.Data = data;
        }
        public Response( string data)
        {
            Status = 200;
            initWithText(data);
        }
        

        protected void initWithText(string text){
            this.Data = System.Text.Encoding.GetEncoding("utf-8").GetBytes(text);
            init();
        }

        protected void init()
        {
            ContentType = "text/html; charset=utf-8";
        }
        // public Response( String name, Hash parameters)
        // {
        //     Status = 200;
        //     var fs = new FileStream("FeintSite/" + Settings.ViewsFolder + name, FileMode.Open, FileAccess.Read);
        //     var reader = new StreamReader(fs, Encoding.UTF8);
        //     var contents = reader.ReadToEnd();
        //     fs.Close();
        //     var template = Template.Parse(contents);
        //     var renderedTemplate = template.Render(parameters);
        //     this.Data = System.Text.Encoding.GetEncoding("utf-8").GetBytes(renderedTemplate);
        // }
        public static Response Redirect( String url)
        {
            var r = new Response() { RedirectUrl = url };
            return r;
        }

    }
}
