
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Threading;

namespace FeintSDK
{
    public class DbBase : DbContext
    {
        private static ConcurrentDictionary<int, DbBase> threadCondextDictionary = new ConcurrentDictionary<int, DbBase>();
        public static DbBase Instance
        {
            get
            {
                DbBase instance = null;
                var success = threadCondextDictionary.TryGetValue(Thread.CurrentThread.ManagedThreadId, out instance);
                if(success)
                    return instance;
                instance = new DbBase();
                threadCondextDictionary.TryAdd(Thread.CurrentThread.ManagedThreadId, instance);
                return instance;
            }
        }

        public static DbSet<TEntity> DbSet<TEntity>() where TEntity : class
        {
            return Instance.Set<TEntity>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (Settings.DatabaseSettings.Type)
            {
                case DbTypes.Sqlite:
                    optionsBuilder.UseSqlite(Settings.DatabaseSettings.ConnectionString);
                    break;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity(typeof(SessionKey));
            modelBuilder.Entity(typeof(SessionProperty));
        }
    }
}