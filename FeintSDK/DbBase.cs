
using Microsoft.EntityFrameworkCore;
namespace FeintSDK
{
    public class DbBase : DbContext
    {
        public static DbBase Instance
        {
            get
            {
                return new DbBase();
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