using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.CompilerServices;


namespace FeintSDK
{
    public class Session
    {
        SessionKey sk;
        public static Random random = new Random();
        static long maxId = -1;
        public Session()
        {
            

        }
        
        public string Start(string key)
        {
            var sks = SessionKey.Find<SessionKey>().Where().Eq("Key", key).Execute();
            if ((sks.Count) == 1)
            {
                sk = sks[0];
                return sk.Key;
            }
            else
                return Start();
        }
       
        public string Start()
        {
            SHA1 sha1 = SHA1.Create();
            if (maxId == -1)
                maxId = SessionKey.Find<SessionKey>().Where().Count();
            string before = DateTime.Now.Ticks.ToString() + random.Next() + "" + maxId;
            maxId++;
            var hashed = SHA1Hash(before)+maxId;
            try
            {
                sk = new SessionKey() { Key = hashed };
                sk.Save();
            }
            catch
            {
            }
            return hashed;
        }
    
        public string GetProperty(string name)
        {
            var props = SessionProperty.Find<SessionProperty>().Where().Eq("owner", sk).And().Eq("Name",name).Execute();

            if (props.Count > 0)
                return props[0].Value;
            else
                return null;
        }
      
        public void SetProperty(string name, string value)
        {
            SessionProperty sp = new SessionProperty() { Name = name, Value = value,Owner=sk };
            sp.Save();
        }
       
        public void UnsetProperty(String name)
        {
            var props = SessionProperty.Find<SessionProperty>().Where().Eq("owner", sk).And().Eq("Name", name).Execute();
            foreach (var p in props)
            {
                p.Remove();
            }
        }
        public String this[string key]
        {
            get
            {
                return this.GetProperty(key);
            }
            set
            {
                SetProperty(key, value);
            }
        }
        public static string SHA1Hash(string text)
        {
            SHA1 sha1 = SHA1.Create();
            sha1.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = sha1.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

    }
}
