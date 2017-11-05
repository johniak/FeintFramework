using Microsoft.EntityFrameworkCore;

namespace FeintSDK
{
    public class BaseModel : IWritable
    {
        public virtual int? Id { get; set; }

        public object Save()
        {
            if (DbBase.Instance.Entry(this).State == EntityState.Detached)
            {
                DbBase.Instance.Add(this);
            }
            else
            {
                DbBase.Instance.Update(this);
            }
            DbBase.Instance.SaveChanges();
            return this;
        }

        public void delete()
        {
            DbBase.Instance.Remove(this);
            DbBase.Instance.SaveChanges();
        }
    }
}