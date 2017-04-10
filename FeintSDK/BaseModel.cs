
namespace FeintSDK
{
    public class BaseModel : IWritable
    {
        public int? Id { get; set; }

        public object Save()
        {
            if (Id != null)
                DbBase.Instance.Update(this);
            else
                DbBase.Instance.Add(this);
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