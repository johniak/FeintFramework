using FeintFramework.Db.Migrator.Fields;
using LinqToDB;
using LinqToDB.Mapping;

namespace FeintFramework.Db;
public class Model
{

}
public class Model<T> : Model where T : Model<T>
{
    [PrimaryKey, Identity, AutoField(PrimaryKey = true)]
    public int? Id { get; set; }

    public static LinqToDB.ITable<T> Objects => FeintFramework.Db.Connections.Connection!.GetTable<T>();
    public virtual void Save()
    {
        if (Id == null)
        {
            Connections.Connection!.Insert((T)this);
        }
        else
        {
            Connections.Connection!.Update((T)this);
        }
    }
}