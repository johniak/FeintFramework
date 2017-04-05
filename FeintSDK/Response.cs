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
        public string MimeType { get; set; }
        private readonly Dictionary<String, String> _headers = new Dictionary<string, string>();
        public Dictionary<String, String> Headers { get { return _headers; } }
        private readonly CookiesSet _cookiesSet = new CookiesSet();
        public CookiesSet Cookies { get { return _cookiesSet; } }
        public bool IsRedirect { get { return RedirectUrl != null; } }
        public string RedirectUrl { get; private set; }

        private Response(Request request)
        {
            initializeByRequest(request);
            Status = 200;
        }
        public Response(Request request, byte[] data)
        {
            initializeByRequest(request);
            Status = 200;
            this.Data = data;
        }
        public Response(Request request, string data)
        {
            initializeByRequest(request);
            Status = 200;
            this.Data = System.Text.Encoding.GetEncoding("utf-8").GetBytes(data);
        }
        // public Response(Request request, String name, Hash parameters)
        // {
        //     initializeByRequest(request);
        //     Status = 200;
        //     var fs = new FileStream("FeintSite/" + Settings.ViewsFolder + name, FileMode.Open, FileAccess.Read);
        //     var reader = new StreamReader(fs, Encoding.UTF8);
        //     var contents = reader.ReadToEnd();
        //     fs.Close();
        //     var template = Template.Parse(contents);
        //     var renderedTemplate = template.Render(parameters);
        //     this.Data = System.Text.Encoding.GetEncoding("utf-8").GetBytes(renderedTemplate);
        // }
        public static Response Redirect(Request request, String url)
        {
            var r = new Response(request) { RedirectUrl = url };
            return r;
        }

        private void initializeByRequest(Request request)
        {
           // _cookiesSet.AddAll(request.Cookies.GetAll());
        }

    }
}
