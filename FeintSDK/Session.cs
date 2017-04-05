using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace FeintSDK
{
    public class Session
    {
        SessionKey sessionKey;
        public static Random random = new Random();
        static long maxId = -1;


        public String Key
        {
            get { return sessionKey.Key; }    
        }

        public Session()
        {
            

        }
        
        public string Start(string key)
        {
            var query =DbBase.DbSet<SessionKey>().Where(sk => sk.Key == key);
            if ((query.Count()) == 1)
            {
                sessionKey = query.First();
                return sessionKey.Key;
            }
            else
                return Start();
        }
       
        public string Start()
        {
            SHA1 sha1 = SHA1.Create();
            if (maxId == -1)
                maxId = DbBase.DbSet<SessionKey>().Count();
            string before = DateTime.Now.Ticks.ToString() + random.Next() + "" + maxId;
            maxId++;
            var hashed = SHA256Hash(before)+maxId;
            try
            {
                sessionKey = new SessionKey() { Key = hashed };
                DbBase.DbSet<SessionKey>().Add(sessionKey);
                DbBase.Instance.SaveChanges();
            }
            catch
            {
            }
            return hashed;
        }

        public string GetProperty(string name)
        {
            var query = DbBase.DbSet<SessionProperty>().Where(sp => sp.Owner == sessionKey && sp.Name == name);
            if (query.Count() <= 0)
                return null;
            return query.First().Value;
        }
      
        public void SetProperty(string name, string value)
        {
            SessionProperty sp = new SessionProperty() { Name = name, Value = value,Owner=sessionKey };
            DbBase.DbSet<SessionProperty>().Add(sp);
            DbBase.Instance.SaveChanges();
        }
       
        public void UnsetProperty(String name)
        {
            var query = DbBase.DbSet<SessionProperty>().Where(sp => sp.Owner == sessionKey && sp.Name == name);
            DbBase.DbSet<SessionProperty>().RemoveRange(query);
            DbBase.Instance.SaveChanges();
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
        public static string SHA256Hash(string text)
        {
            var sha256 = SHA256.Create();
            byte[] result = sha256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            StringBuilder strBuilder = new StringBuilder();
            foreach(var item in result)
            {
                strBuilder.Append(item.ToString("x2"));
            }
            return strBuilder.ToString();
        }

    }
    
    // class SessionContext : DbContext
    // {
    //     public DbSet<SessionKey> SessionKeys { get; set; }
    //     public DbSet<SessionProperty> SessionProperties { get; set; }

    //     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     {
    
    //         optionsBuilder.UseSqlite("Data Source=session.db");
    //     }

    // }
}
