using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;

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
                if (success)
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
            FeintSetup.CallSetup();
            switch (Settings.DatabaseSettings.Type)
            {
                case DbTypes.Sqlite:
                    optionsBuilder.UseSqlite(Settings.DatabaseSettings.ConnectionString);
                    break;
                case DbTypes.PosgreSQL:
                    optionsBuilder.UseNpgsql(Settings.DatabaseSettings.ConnectionString);
                    break;
                case DbTypes.PostGIS:
                    optionsBuilder.UseNpgsql(Settings.DatabaseSettings.ConnectionString, o => o.UseNetTopologySuite());
                    break;

            }
        }

        protected async override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Settings.DatabaseSettings.Type == DbTypes.PostGIS)
            {
                modelBuilder.HasPostgresExtension("postgis");
            }
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allModels = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseModel)));
                allModels.AddRange(types);
            }
            foreach (var type in allModels)
            {
                modelBuilder.Entity(type);
            }
        }
    }
}