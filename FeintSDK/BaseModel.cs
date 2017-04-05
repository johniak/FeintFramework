
namespace FeintSDK
{
    public class BaseModel : IModelWritable
    {
        public int? Id { get; set; }

        public void Save()
        {
            if (Id != null)
                DbBase.Instance.Update(this);
            else
                DbBase.Instance.Add(this);
            DbBase.Instance.SaveChanges();
        }

        public void delete(){
            DbBase.Instance.Remove(this);
            DbBase.Instance.SaveChanges();
        }
    }
}