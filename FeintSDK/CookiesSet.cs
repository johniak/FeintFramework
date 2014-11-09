using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    public class CookiesSet :IEnumerable<Cookie>
    {
        private readonly List<Cookie> cookies = new List<Cookie>();

        public Cookie this[String key]
        {
            get
            {
                return cookies.First(cookie => cookie.Name == key);
            }
            set
            {
                cookies[cookies.FindIndex(cookie => cookie.Name == key)] = value;
            }
        }

        public void SetCookie(Cookie cookie)
        {
            var index = cookies.IndexOf(cookie);
            if (index < 0)
            {
                cookies.Add(cookie);
                return;
            }
            cookies[index] = cookie;
        }

        public bool IsCookieExist(String name)
        {
            return cookies.Any(cookie => cookie.Name == name);
        }

        public bool IsCookieExist(String name,String path)
        {
            return cookies.Any(cookie => cookie.Name == name&&cookie.Path==path);
        }

        public void RemoveCookies(String name)
        {
            cookies.RemoveAll(cookie => cookie.Name == name);
        }

        public void RemoveCookie(String name, String path)
        {
            cookies.RemoveAll(cookie => cookie.Name == name && cookie.Path == path);
        }

        public Cookie GetCookie(String name, String path)
        {
            return cookies.First(cookie => cookie.Name == name&&cookie.Path==path);
        }

        public IEnumerable<Cookie> GetCookies(String name)
        {
            return cookies.Where(cookie => cookie.Name == name);
        }

        public List<Cookie> GetAll()
        {
            return cookies;
        }  

        public void Clear()
        {
            cookies.Clear();
        }

        public void Set(Cookie cookie)
        {
            if (IsCookieExist(cookie.Name, cookie.Path))
            {
                var index = cookies.IndexOf(cookie);
                cookies[index] = cookie;
            }
            else
            {
                cookies.Add(cookie);
            }
        }

        public void AddAll(List<Cookie> cookies)
        {
            this.cookies.AddRange(cookies);
        }

        public IEnumerator<Cookie> GetEnumerator()
        {
            return cookies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
