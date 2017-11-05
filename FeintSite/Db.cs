using FeintSDK;
using Microsoft.EntityFrameworkCore;

namespace FeintSite
{
    class Db : DbBase
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           // modelBuilder.Entity(typeof(ExampleModel));
        }
    }
}